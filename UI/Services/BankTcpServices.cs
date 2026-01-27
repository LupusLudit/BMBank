using System.IO;
using System.Net.Sockets;
using System.Text;

namespace UI;

public class BankTcpServices
{
    private string ip;
    private int port;
    
    public BankTcpServices(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
    }
    
    public async Task<bool> IsServerOnline()
    {
        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(ip, port);
            var timeoutTask = Task.Delay(2000);

            if (await Task.WhenAny(connectTask, timeoutTask) == timeoutTask)
            {
                return false;
            }

            return client.Connected;
        }
        catch { return false; }
    }
    
    public async Task<int> GetClientCount()
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(ip, port);

            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            writer.WriteLine("UI-BN");
            string? response = await reader.ReadLineAsync();

            if (response != null)
            {
                var parts = response.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 2 && int.TryParse(parts[1], out int count))
                {
                    return count;
                }
            }
        }
        catch { }

        return 0;
    }
}