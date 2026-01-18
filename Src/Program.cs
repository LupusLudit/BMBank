using BMBank.Src.Common;
using BMBank.Src.Network;

namespace BMBank.Src
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            string? localIp = IPAddressObtainer.GetLocalIPv4Address();

            if (localIp != null)
            {
                string startingMessage = $"BMBank server running at: {localIp}";
                Console.WriteLine(startingMessage);
                Logger.Info(startingMessage);

                Server server = new Server(65526);

                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    Logger.Info("Shutting down BMBank Server...");
                    server.Stop();
                };

                await server.StartAsync();
            }
            else
            {
                Logger.Error("Couldn't obtain local ip address.");
                Console.WriteLine("Couldn't obtain local ip address, server was not started.");
            }
        }
    }
}