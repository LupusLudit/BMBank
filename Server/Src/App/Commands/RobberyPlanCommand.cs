using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands;

public class RobberyPlanCommand : ICommand
{
    public string Key => "RP";
    private const int timeout = 5000;
    private const int maxParallel = 10;

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

            sum += bank.Money;
            harmed += bank.Clients;
            selected.Add(bank);
        }

        return $"RP To reach {targetAmount} it is necessary to rob " +
               $"{string.Join(", ", selected.Select(b => b.Ip))} " +
               $"and {harmed} clients will be harmed.";
    }
}