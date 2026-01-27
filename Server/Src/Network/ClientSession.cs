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

        /// <summary>
        /// Initializes a new client session for the connected TCP client.
        /// </summary>
        /// <param name="client">The TCP client representing the connection.</param>
        /// <param name="connection">The SQL connection used for command handling and logging.</param>
        public ClientSession(TcpClient client, SqlConnection connection)
        {
            this.client = client;
            commandHandler = new CommandHandler(connection);
            logDao = new ClientCommandLogDAO(connection);
        }

        /// <summary>
        /// Handles all incoming requests from the client asynchronously.
        /// Processes commands, handles timeouts, and logs all executed commands except UI polling.
        /// Exceptions are caught and logged using <see cref="Logger"/>.
        /// </summary>
        /// <param name="serverCancellationToken">Cancellation token to stop processing when the server is shutting down.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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
                    string? request;
                    try
                    {
                        request = await reader.ReadLineAsync();
                    }
                    catch (IOException ex)
                    {
                        Logger.Warning($"IO exception while reading from [{clientIp}]: {ex.Message}");
                        break;
                    }

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

                    string response;
                    try
                    {
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

                        response = await commandTask;
                        await writer.WriteLineAsync(response);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error executing command from [{clientIp}]: {ex.Message}");
                        response = "ER executing command";
                        await writer.WriteLineAsync(response);
                    }
                    
                    string commandKey = request.Substring(0, 2).ToUpper();
                    string arguments = "";

                    if (request.Length > 2)
                    {
                        arguments = request.Substring(2).Trim();
                    }
                    
                    try
                    {
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
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to log command from [{clientIp}]: {ex.Message}");
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
                try
                {
                    client.Close();
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to close client socket [{clientIp}]: {ex.Message}");
                }
                Logger.Info($"Session Closed for [{clientIp}]");
            }
        }
    }
}