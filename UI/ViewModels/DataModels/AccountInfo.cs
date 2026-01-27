namespace UI.ViewModels.DataModels;

public class AccountInfo
{
    public int AccountNumber { get; set; }
    public long Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}