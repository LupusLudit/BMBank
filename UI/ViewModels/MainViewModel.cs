using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Microsoft.Data.SqlClient;
using System.Windows.Threading;
using UI.Services;

namespace UI.ViewModels;

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
    
    private int clientCount;
    public string ConnectedClientsText
    {
        get
        {
            return $"Total bank clients: {clientCount}";
        }
    }

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
        if (int.TryParse(ConfigurationManager.AppSettings["ServerPort"], out int p))
        {
            port = p;
        }
        
        tcp = new BankTcpServices(ip, port);
        
        string dataSource = ConfigurationManager.AppSettings["DataSource"];
        string database = ConfigurationManager.AppSettings["Database"];
        string login = ConfigurationManager.AppSettings["Login"];
        string password = ConfigurationManager.AppSettings["Password"];
        
        var conn = new SqlConnection($"Server={dataSource};Database={database};User Id={login};Password={password};TrustServerCertificate=True");
        
        db = new DatabaseService(conn);

        var dispatcherTimer = new DispatcherTimer();
        dispatcherTimer.Interval = TimeSpan.FromSeconds(3);
        dispatcherTimer.Tick += async (s, e) => await RefreshAll();
        dispatcherTimer.Start();
    }
    
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
        foreach (var client in db.GetClientInfos())
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

        clientCount = await tcp.GetClientCount();
        OnPropertyChanged(nameof(ConnectedClientsText));
    }


    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}