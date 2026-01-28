namespace UI.ViewModels.DataModels
{
    /// <include file='../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AccountInfo"]/*'/>
    public class AccountInfo
    {
        public int AccountNumber { get; set; }
        public long Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
    }
}