using BMBank.Src.App;
using BMBank.Src.Common;
using Microsoft.Data.SqlClient;
using System.Net.Sockets;
using System.Text;
using BMBank.Src.DatabaseInteraction.Core.DAO;

namespace BMBank.Src.Network
{
    public class ClientSession
    {
        private TcpClient client;
        private CommandHandler commandHandler;
        private const int timeoutInMs = 5000;
        private ClientCommandLogDAO logDao;

        public ClientSession(TcpClient client, SqlConnection connection)
        {
            this.client = client;
            commandHandler = new CommandHandler(connection);
            logDao = new ClientCommandLogDAO(connection);
        }

        public async Task HandleAsync(CancellationToken serverCancellationToken)
        {
            string? clientIp = client.Client.RemoteEndPoint?.ToString();
            Logger.Info($"New Session started at [{clientIp}]");

            try
            {
                client.ReceiveTimeout = timeoutInMs;
                client.SendTimeout = timeoutInMs;

                NetworkStream stream = client.GetStream();

                using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                using StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                while (!serverCancellationToken.IsCancellationRequested)
                {
                    string? request = await reader.ReadLineAsync();

                    if (request == null)
                    {
                        Logger.Info($"Client disconnected [{clientIp}]");
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(request))
                        continue;
                    
                    bool isUiRequest = request.StartsWith("UI-");

                    if (isUiRequest)
                    {
                        request = request.Substring(3);
                    }
                    
                    if (!isUiRequest)
                    {
                        Logger.Info($"Received request from [{clientIp}]: {request}");
                    }

                    Task<string> commandTask = Task.Run(() =>
                        SafeExecutor.Execute(() => commandHandler.ProcessCommand(request))
                    );

                    Task timeoutTask = Task.Delay(timeoutInMs, serverCancellationToken);

                    Task completed = await Task.WhenAny(commandTask, timeoutTask);

                    if (completed == timeoutTask)
                    {
                        Logger.Warning($"Command timeout for [{clientIp}]");
                        await writer.WriteLineAsync("ER Session timeout");
                        continue;
                    }

                    string response = await commandTask;
                    await writer.WriteLineAsync(response);
                    
                    string commandKey = request.Substring(0, 2).ToUpper();
                    string arguments = "";

                    if (request.Length > 2)
                    {
                        arguments = request.Substring(2).Trim();
                    }
                    
                    if (!isUiRequest)
                    {
                        logDao.Insert(
                            clientIp ?? "UNKNOWN",
                            commandKey,
                            arguments,
                            response.StartsWith("ER") ? "ERROR" : "OK"
                        );
                    }
                }
            }
            catch (IOException ex)
            {
                Logger.Warning($"IO timeout or disconnect [{clientIp}]: {ex.Message}");
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