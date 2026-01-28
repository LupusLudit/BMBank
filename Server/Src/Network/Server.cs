using BMBank.Src.Common;
using Microsoft.Data.SqlClient;
using System.Net.Sockets;

namespace BMBank.Src.Network
{
    /// <include file='../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="Server"]/*'/>
    public class Server
    {
        private TcpListener listener;
        private CancellationTokenSource cancellationTokenSource;
        private SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the Server class.
        /// </summary>
        /// <param name="port">The TCP port on which the server will listen for connections.</param>
        /// <param name="connection">The SQL connection used for database interactions.</param>
        public Server(int port, SqlConnection connection)
        {
            listener = new TcpListener(System.Net.IPAddress.Any, port);
            this.connection = connection;

            cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Starts the server and begins accepting client connections asynchronously.
        /// Each connection is handled in a separate task with a ClientSession.
        /// </summary>
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
