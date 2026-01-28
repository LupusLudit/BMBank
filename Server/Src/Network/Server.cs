using BMBank.Src.Common;
using Microsoft.Data.SqlClient;
using System.Net.Sockets;

namespace BMBank.Src.Network
{
    /// <include file='../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="Server"]/*'/>
    public class Server
    {
        private TcpListener listener;
        private CancellationTokenSource cancellationTokenSource = new();
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

                    _ = Task.Run(async 
                        () =>
                        {
                            try
                            {
                                ClientSession session = new ClientSession(client, connection);
                                await session.HandleAsync(cancellationTokenSource.Token);
                            }
                            catch (Exception e)
                            {
                                Logger.Error($"Session error: {e.Message}");
                            }
                        }
                    );
                }
            }
            catch (ObjectDisposedException)
            {
                Logger.Info("Server stopping...");
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
