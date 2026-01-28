using System.Collections.Concurrent;
using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
<<<<<<< HEAD
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="RobberyPlanCommand"]/*'/>
    public class RobberyPlanCommand : ICommand
    {
        public string Key => "RP";
        private const int timeout = 5000;
        private const int maxParallel = 10;

        /// <summary>
        /// Executes the Robbery Plan command (RP).
        /// </summary>
        /// <param name="arguments">
        /// Target amount of money to be collected.
        /// </param>
        /// <returns>
        /// A robbery plan describing which banks must be robbed and how many
        /// clients will be harmed, or an error message if the operation fails or times out.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown if the provided argument is not a valid number.
        /// </exception>
        public string Execute(string arguments)
        {
            if (!long.TryParse(arguments.Trim(), out long targetAmount))
            {
                throw new FormatException("ER Invalid format. Usage: RP <number>");
            }

            using var cts = new CancellationTokenSource(timeout);

            try
            {
                return ExecuteInternal(targetAmount, cts.Token);
            }
            catch (Exception e)
            {
                return "ER Robbery plan timeout";
            }
        }

        /// <summary>
        /// Internal execution logic for the robbery plan.
        /// </summary>
        /// <param name="targetAmount">
        /// Target amount of money to be collected.
        /// </param>
        /// <param name="token">
        /// Cancellation token used to enforce execution timeout.
        /// </param>
        /// <returns>
        /// A formatted robbery plan result, or an informational message
        /// if no banks are available in the network.
        /// </returns>
        private string ExecuteInternal(long targetAmount, CancellationToken token)
        {
            string localIp = IPAddressObtainer.GetLocalIPv4Address();

            var results = new ConcurrentBag<BankNetworkExplorer.BankInfo>();
            var semaphore = new SemaphoreSlim(maxParallel);

            var tasks = BankNetworkExplorer.DiscoverBanks().Where(ip => ip != localIp).Select(ip => Task.Run(async () =>
            {
                await semaphore.WaitAsync(token);
                try
                {
                    token.ThrowIfCancellationRequested();

                    string ba = BankProxyClient.Forward(ip, "BA");
                    string bn = BankProxyClient.Forward(ip, "BN");

                    if (!ba.StartsWith("BA ") || !bn.StartsWith("BN "))
                    {
                        return;
                    }

                    long money = long.Parse(ba.Split(' ')[1]);
                    int clients = int.Parse(bn.Split(' ')[1]);

                    results.Add(new BankNetworkExplorer.BankInfo(ip, money, clients));
                }
                catch
                {
                }
                finally
                {
                    semaphore.Release();
                }
            }, token)).ToList();

            Task.WaitAll(tasks.ToArray(), token);

            if (results.Count == 0)
            {
                return "RP No banks available in network";
            }

            long sum = 0;
            int harmed = 0;
            var selected = new List<BankNetworkExplorer.BankInfo>();

            foreach (var bank in results.OrderBy(b => b.Clients).ThenByDescending(b => b.Money))
            {
                if (sum >= targetAmount)
                {
                    break;
                }

=======
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
>>>>>>> 48d90d1ccacc2ec931ab9ab9039ef5a2accccd61
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
