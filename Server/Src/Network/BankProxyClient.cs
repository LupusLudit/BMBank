using System.Net.Sockets;
using System.Text;

namespace BMBank.Src.Network;

public static class BankProxyClient
{
    private const int Timeout = 60000;

    /// <summary>
    /// Forwards a command to a remote bank node.
    /// </summary>
    /// <param name="targetIp">The target bank IP address.</param>
    /// <param name="command">The command to forward.</param>
    /// <returns>
    /// The response from the remote bank node, or "ER No bank found" if no node responded.
    /// </returns>
    public static async Task<string?> ForwardAsync(string targetIp, string command, CancellationToken token)
    {
        for (int port = NetworkConfig.PortFrom; port <= NetworkConfig.PortTo; port++)
        {
            try
            {
                using var client = new TcpClient();

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
                cts.CancelAfter(Timeout);

                await client.ConnectAsync(targetIp, port, cts.Token);

                using var stream = client.GetStream();
                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                await writer.WriteLineAsync(command);
                var response = await reader.ReadLineAsync();

                if (!string.IsNullOrEmpty(response))
                    return response;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch
            {
            }
        }

        return null;
    }
}