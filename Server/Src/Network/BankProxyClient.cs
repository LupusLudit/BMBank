using System.Net.Sockets;
using System.Text;

namespace BMBank.Src.Network;

public static class BankProxyClient
{
    private const int Timeout = 5000;

    public static string Forward(string targetIp, string command)
    {
        for (int port = NetworkConfig.PortFrom; port <= NetworkConfig.PortTo; port++)
        {
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(targetIp, port);

                if (!connectTask.Wait(Timeout))
                {
                    continue;
                }

                using var stream = client.GetStream();
                stream.ReadTimeout = Timeout;
                stream.WriteTimeout = Timeout;

                using var reader = new StreamReader(stream, Encoding.UTF8);
                using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                writer.WriteLine("BC");
                var bcResponse = reader.ReadLine();

                if (bcResponse == null || !bcResponse.StartsWith("BC "))
                {
                    continue;
                }

                writer.WriteLine(command);
                var response = reader.ReadLine();

                if (response != null)
                {
                    return response;
                }
            }
            catch { }
        }

        return "ER No bank found";
    }
}