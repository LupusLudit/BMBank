using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Microsoft.Data.SqlClient;
using System.Windows.Threading;
using UI.Services;
using UI.ViewModels.DataModels;

namespace UI.ViewModels
{
    /// <include file='../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="MainViewModel"]/*'/>
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private BankTcpServices tcp;
        private DatabaseService db;

        public ObservableCollection<ClientInfo> Clients { get; } = new();
        public ObservableCollection<AccountInfo> ActiveAccounts { get; } = new();
        public ObservableCollection<AccountInfo> ClosedAccounts { get; } = new();

        private string serverStatus = "UNKNOWN";
        public string ServerStatus
        {
            get
            {
                return serverStatus;
            }
            set
            {
                serverStatus = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ServerStatusColor));
            }
        }

        public Brush ServerStatusColor
        {
            get
            {
                if (ServerStatus == "ONLINE")
                {
                    return Brushes.Green;
                }
                else
                {
                    return Brushes.Red;
                }
            }

        }

        /// <summary>
        /// Initializes the ViewModel.
        /// Sets up TCP connection, database service, and starts a timer for automatic data refresh every 3 seconds.
        /// </summary>
        /// <exception cref="SqlException">Thrown if the database connection cannot be opened.</exception>
        /// <exception cref="FormatException">Thrown if the server port in the configuration is invalid.</exception>
        public MainViewModel()
        {
            string ip;

            if (ConfigurationManager.AppSettings["ServerIp"] != null)
            {
                ip = ConfigurationManager.AppSettings["ServerIp"];
            }
            else
            {
                ip = "127.0.0.1";
            }

            int port = 65525;
            if (!int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out port))
            {
                throw new FormatException("Invalid ServerPort in configuration.");
            }

            tcp = new BankTcpServices(ip, port);

            string dataSource = ConfigurationManager.AppSettings["DataSource"];
            string database = ConfigurationManager.AppSettings["Database"];
            string login = ConfigurationManager.AppSettings["Login"];
            string password = ConfigurationManager.AppSettings["Password"];

            var conn = new SqlConnection($"Server={dataSource};Database={database};User Id={login};Password={password};TrustServerCertificate=True;"); // Add "Trusted_Connection=True;" for local testing

            db = new DatabaseService(conn);

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = TimeSpan.FromSeconds(3);
            dispatcherTimer.Tick += async (s, e) => await RefreshAll();
            dispatcherTimer.Start();
        }

        /// <summary>
        /// Refreshes all data: server status, active accounts, closed accounts, clients, and client count.
        /// </summary>
        private async Task RefreshAll()
        {
            bool online = await tcp.IsServerOnline();
            if (online)
            {
                ServerStatus = "ONLINE";
            }
            else
            {
                ServerStatus = "OFFLINE";
            }

            ActiveAccounts.Clear();
            foreach (var a in db.GetActiveAccounts())
            {
                ActiveAccounts.Add(a);
            }

            Clients.Clear();
            foreach (var client in db.GetClientInformation())
            {
                Clients.Add(client);
            }

            ClosedAccounts.Clear();
            foreach (var a in db.GetClosedAccounts())
            {
                ClosedAccounts.Add(a);
            }

            if (!online)
            {
                return;
            }
        }

        /// <summary>Raises the PropertyChanged event for UI bindings.</summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}