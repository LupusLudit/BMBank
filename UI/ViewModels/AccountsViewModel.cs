namespace UI.ViewModels;

public class ActiveAccountViewModel
{
    public int AccountNumber { get; set; }
    public long Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ClosedAccountViewModel : ActiveAccountViewModel
{
    public DateTime ClosedAt { get; set; }
}