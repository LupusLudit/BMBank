using BMBank.Src.Common;
using Microsoft.Data.SqlClient;
using System.Net.Sockets;

namespace BMBank.Src.Network
{
    public class Server
    {
        private TcpListener listener;
        private CancellationTokenSource cancellationTokenSource;
        private SqlConnection connection;

        public Server(int port, SqlConnection connection)
        {
            listener = new TcpListener(System.Net.IPAddress.Any, port);
            this.connection = connection;

            cancellationTokenSource = new CancellationTokenSource();
        }

        public async Task StartAsync()
        {
            listener.Start();
            Logger.Info("Server started.");

            try
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    await Task.Run(
                        () =>
                        {
                            ClientSession session = new ClientSession(client, connection);
                            return session.HandleAsync(cancellationTokenSource.Token);
                        },
                        cancellationTokenSource.Token
                    );
                }
            }
            catch (Exception ex) when (cancellationTokenSource.IsCancellationRequested)
            {
                Logger.Info("Server stopping...");
            }
            catch (Exception ex)
            {
                Logger.Error($"Server encountered an error: {ex.Message}");
            }
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
            listener.Stop();
            Logger.Info("Server stopped.");
        }
    }
}
