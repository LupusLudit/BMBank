using BMBank.Src.App;
using BMBank.Src.Common;
using System.Net.Sockets;
using System.Text;

namespace BMBank.Src.Network
{
    public class ClientSession
    {
        private TcpClient client;
        private CommandHandler commandHandler;
        private const int timeoutInMs = 5000;

        public ClientSession(TcpClient client)
        {
            this.client = client;
            commandHandler = new CommandHandler();
        }

        public async Task HandleAsync(CancellationToken serverCancellationToken)
        {
            string? clientIp = client.Client.RemoteEndPoint?.ToString();
            Logger.Info($"New Session started at [{clientIp}]");

            try
            {
                NetworkStream stream = client.GetStream();

                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                {
                    while (client.Connected && !serverCancellationToken.IsCancellationRequested)
                    {
                        Task<string?> readTask = reader.ReadLineAsync();
                        Task timeoutTask = Task.Delay(timeoutInMs, serverCancellationToken);
                        Task completedTask = await Task.WhenAny(readTask, timeoutTask);

                        if (completedTask == timeoutTask)
                        {
                            Logger.Warning($"Session timeout for [{clientIp}]");
                            await writer.WriteLineAsync("Session Timeout");
                        }

                        string? request = await readTask;
                        if (request == null) break;
                        if (string.IsNullOrWhiteSpace(request)) continue;

                        Logger.Info($"Received request from [{clientIp}]: {request}");

                        string response = SafeExecutor.Execute(() => commandHandler.ProcessCommand(request));

                        await writer.WriteLineAsync(response);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Error($"Network Error at [{clientIp}]: {ex.Message}");
            }
            finally
            {
                client.Close();
                Logger.Info($"Session Closed for [{clientIp}]");
            }
        }
    }
}