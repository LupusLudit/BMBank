using BMBank.Src.Common;
using BMBank.Src.Network;
using BookOrg.Src.Logic.Connection;
using Microsoft.Data.SqlClient;

namespace BMBank.Src
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                IConnectionFactory connectionFactory = new SqlServerConnectionFactory();

                Console.WriteLine("Attempting to connect to the database...");
                SqlConnection? connection = await Task.Run(() => connectionFactory.CreateConnection());

                if (connection == null)
                {
                    Logger.Error("Failed to connect to the database.");
                    Console.WriteLine($"Failed to connect to the database.");
                    return;
                }

                string? localIp = IPAddressObtainer.GetLocalIPv4Address();

                if (localIp == null)
                {
                    Logger.Error("Couldn't obtain local ip address.");
                    Console.WriteLine("Couldn't obtain local ip address, server was not started.");
                    return;
                }
                else
                {
                    string startingMessage = $"BMBank server running at: {localIp}";
                    Console.WriteLine(startingMessage);
                    Logger.Info(startingMessage);

                    Server server = new Server(65526, connection);

                    Console.CancelKeyPress += (sender, eventArgs) =>
                    {
                        eventArgs.Cancel = true;
                        Logger.Info("Shutting down BMBank Server...");
                        server.Stop();
                    };

                    await server.StartAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Fatal error: {ex.Message}");
                Console.WriteLine($"Fatal error: {ex.Message}");
            }
        }
    }
}