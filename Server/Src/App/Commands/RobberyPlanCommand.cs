using System.Collections.Concurrent;
using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="RobberyPlanCommand"]/*'/>
    public class RobberyPlanCommand : IAsyncCommand
    {
        public string Key => "RP";
        public string Execute(string arguments)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ExecuteAsync(string arguments, CancellationToken token)
        {
            if (!long.TryParse(arguments.Trim(), out long targetAmount))
                throw new FormatException("ER Invalid format. Usage: RP <number>");

            string localIp = IPAddressObtainer.GetLocalIPv4Address();
            var results = new ConcurrentBag<BankNetworkExplorer.BankInfo>();
            var semaphore = new SemaphoreSlim(10);

            var tasks = BankNetworkExplorer.DiscoverBanks()
                .Where(ip => ip != localIp)
                .Select(async ip =>
                {
                    await semaphore.WaitAsync(token);
                    try
                    {
                        var baTask = BankProxyClient.ForwardAsync(ip, "BA", token);
                        var bnTask = BankProxyClient.ForwardAsync(ip, "BN", token);
                        await Task.WhenAll(baTask, bnTask);

                        string? ba = await baTask;
                        string? bn = await bnTask;
                        if (ba == null || bn == null) return;
                        if (!ba.StartsWith("BA ") || !bn.StartsWith("BN ")) return;

                        long money = long.Parse(ba.Split(' ')[1]);
                        int clients = int.Parse(bn.Split(' ')[1]);
                        results.Add(new BankNetworkExplorer.BankInfo(ip, money, clients));
                    }
                    catch { }
                    finally
                    {
                        semaphore.Release();
                    }
                });

            await Task.WhenAll(tasks);

            if (results.Count == 0)
                return "RP No banks available in network";

            long sum = 0;
            int harmed = 0;
            var selected = new List<BankNetworkExplorer.BankInfo>();

            foreach (var bank in results.OrderBy(b => b.Clients).ThenByDescending(b => b.Money))
            {
                if (sum >= targetAmount) break;
                sum += bank.Money;
                harmed += bank.Clients;
                selected.Add(bank);
            }

            return $"RP To reach {targetAmount} it is necessary to rob " +
                   $"{string.Join(", ", selected.Select(b => b.Ip))} " +
                   $"and {harmed} clients will be harmed.";
        }
    }
}