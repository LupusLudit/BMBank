using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace UI.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    
    public string ServerStatus { get; set; } = "Server: ONLINE";
    public Brush ServerStatusColor { get; set; } = Brushes.Green;
    public string ConnectedClientsText => $"Connected clients: {Clients.Count}";
    
    public ObservableCollection<ClientViewModel> Clients { get; } = new ObservableCollection<ClientViewModel>();
    public ObservableCollection<ActiveAccountViewModel> ActiveAccounts { get; } = new ObservableCollection<ActiveAccountViewModel>();
    public ObservableCollection<ClosedAccountViewModel> ClosedAccounts { get; } = new ObservableCollection<ClosedAccountViewModel>();
    public ObservableCollection<LogsViewModel> Logs { get; } = new ObservableCollection<LogsViewModel>();

    public MainViewModel()
    {
        Clients.Add(new ClientViewModel
        {
            Ip = "10.1.2.15",
            LastCommand = "AC",
            Timestamp = DateTime.Now.ToString("HH:mm:ss"),
            Status = "Active"
        });
        
        ActiveAccounts.Add(new ActiveAccountViewModel
        {
            AccountNumber = 10001,
            Balance = 5000,
            CreatedAt = DateTime.UtcNow
        });

        ClosedAccounts.Add(new ClosedAccountViewModel
        {
            AccountNumber = 10000,
            Balance = 0,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            ClosedAt = DateTime.UtcNow
        });

        Logs.Add(new LogsViewModel
        {
            Time = DateTime.Now.ToString("HH:mm:ss"),
            Ip = "10.1.2.15",
            Level = "INFO",
            Message = "Client connected"
        });
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