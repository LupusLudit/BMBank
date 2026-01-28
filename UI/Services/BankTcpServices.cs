using System.IO;
using System.Net.Sockets;
using System.Text;

namespace UI
{
    /// <include file='../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="BankTcpServices"]/*'/>
    public class BankTcpServices
    {
        private string ip;
        private int port;

        public BankTcpServices(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }

        /// <summary>
        /// Checks whether the bank server is online.
        /// </summary>
        /// <returns>True if the server is reachable and connected, otherwise false.</returns>
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

        /// <summary>
        /// Retrieves the number of total clients bank has.
        /// </summary>
        /// <returns>The total number of clients, or 0 if the server is unreachable or response is invalid.</returns>
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
}