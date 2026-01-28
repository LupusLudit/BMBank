using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;

/// <include file='../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="OnDemandNetworkScanner"]/*'/>
public class OnDemandNetworkScanner
{
    private readonly int portFrom;
    private readonly int portTo;
    private readonly TimeSpan timeout;
    private readonly int maxParallel;

    public OnDemandNetworkScanner(int portFrom, int portTo, TimeSpan timeout, int maxParallel = 200)
    {
        this.portFrom = portFrom;
        this.portTo = portTo;
        this.timeout = timeout;
        this.maxParallel = maxParallel;
    }

    /// <summary>
    /// Scans the specified IP addresses and port range to discover available banks.
    /// </summary>
    /// <param name="ips">Collection of IP addresses to scan.</param>
    /// <param name="token">Cancellation token used to cancel the scan.</param>
    /// <returns>
    /// A list of discovered banks containing their address, port, and basic status information.
    /// </returns>
    public async Task<List<DiscoveredBank>> ScanAsync(IEnumerable<string> ips, CancellationToken token)
    {
        var results = new ConcurrentBag<DiscoveredBank>();
        var semaphore = new SemaphoreSlim(maxParallel);

        var tasks =
            from ip in ips
            from port in Enumerable.Range(portFrom, portTo - portFrom + 1)
            select ProbeAsync(ip, port, results, semaphore, token);

        await Task.WhenAll(tasks);
        return results.ToList();
    }

    /// <summary>
    /// Probes a single IP address and port for a bank node.
    /// If a valid bank protocol response is received,
    /// the bank is added to the result collection.
    /// </summary>
    /// <param name="ip">Target IP address.</param>
    /// <param name="port">Target port number.</param>
    /// <param name="results">
    /// Thread-safe collection used to store discovered banks.
    /// </param>
    /// <param name="semaphore">
    /// Semaphore limiting the number of parallel probe operations.
    /// </param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>A task representing the asynchronous probe operation.</returns>
    private async Task ProbeAsync(string ip, int port, ConcurrentBag<DiscoveredBank> results, SemaphoreSlim semaphore, CancellationToken token)
    {
        await semaphore.WaitAsync(token);
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.CancelAfter(timeout);

            using var client = new TcpClient();
            await client.ConnectAsync(ip, port, cts.Token);

            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };
            
            await writer.WriteLineAsync("BC");
            var bc = await reader.ReadLineAsync();
            if (bc is null || !bc.StartsWith("BC "))
                return;
            
            await writer.WriteLineAsync("BA");
            var ba = await reader.ReadLineAsync();
            if (ba is null || !ba.StartsWith("BA "))
                return;
            
            await writer.WriteLineAsync("BN");
            var bn = await reader.ReadLineAsync();
            if (bn is null || !bn.StartsWith("BN "))
                return;

            long total = long.Parse(ba.Split(' ')[1]);
            int clients = int.Parse(bn.Split(' ')[1]);

            results.Add(new DiscoveredBank(ip, port, total, clients));
        }
        catch
        {
        }
        finally
        {
            semaphore.Release();
        }
    }
    
    /// <summary>
    /// Represents a discovered bank node and its basic status information.
    /// </summary>
    /// <param name="Ip">IP address of the bank.</param>
    /// <param name="Port">Port on which the bank is listening.</param>
    /// <param name="Total">Total amount of money stored in the bank.</param>
    /// <param name="Clients">Number of connected clients.</param>
    public record DiscoveredBank(string Ip, int Port, long Total, int Clients);
}
