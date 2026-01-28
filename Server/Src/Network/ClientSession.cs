using BMBank.Src.App;
using BMBank.Src.Common;
using Microsoft.Data.SqlClient;
using System.Net.Sockets;
using System.Text;
using BMBank.Src.DatabaseInteraction.Core.DAO;

namespace BMBank.Src.Network;

public class ClientSession
{
<<<<<<< HEAD
    /// <include file='../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="ClientSession"]/*'/>
    public class ClientSession
=======
    private readonly TcpClient client;
    private readonly CommandHandler commandHandler;
    private const int timeoutInMs = 60000;
    private readonly ClientCommandLogDAO logDao;

    public ClientSession(TcpClient client, SqlConnection connection)
>>>>>>> 48d90d1ccacc2ec931ab9ab9039ef5a2accccd61
    {
        this.client = client;
        commandHandler = new CommandHandler(connection);
        logDao = new ClientCommandLogDAO(connection);
    }

    public async Task HandleAsync(CancellationToken serverToken)
    {
        string? clientIp = client.Client.RemoteEndPoint?.ToString();
        Logger.Info($"New Session started at [{clientIp}]");

        try
        {
            client.ReceiveTimeout = timeoutInMs;
            client.SendTimeout = timeoutInMs;

            using NetworkStream stream = client.GetStream();
            using StreamReader reader = new(stream, Encoding.UTF8);
            using StreamWriter writer = new(stream, Encoding.UTF8) { AutoFlush = true };

            while (!serverToken.IsCancellationRequested)
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

                if (string.IsNullOrWhiteSpace(request))
                    continue;

                if (request == null)
                {
                    Logger.Info($"Client disconnected [{clientIp}]");
                    break;
                }

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
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(serverToken);
                    cts.CancelAfter(timeoutInMs);

                    response = await SafeExecutor.ExecuteAsync(async () =>
                    {
                        return await commandHandler.ProcessCommandAsync(request, cts.Token);
                    });
                }
                catch (OperationCanceledException)
                {
                    response = "ER Session timeout";
                }

                await writer.WriteLineAsync(response);

                if (!isUiRequest)
                {
                    string commandKey = request.Substring(0, 2).ToUpper();
                    string arguments = request.Length > 2 ? request.Substring(2).Trim() : "";

                    try
                    {
                        logDao.Insert(
                            clientIp ?? "UNKNOWN",
                            commandKey,
                            arguments,
                            response.StartsWith("ER") ? "ERROR" : "OK"
                        );
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to log command from [{clientIp}]: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error($"Network Error at [{clientIp}]: {ex.Message}");
        }
        finally
        {
            try { client.Close(); } catch { }
            Logger.Info($"Session Closed for [{clientIp}]");
        }
    }
}
