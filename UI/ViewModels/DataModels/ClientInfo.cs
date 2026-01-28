namespace UI.ViewModels.DataModels
{
    /// <include file='../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="ClientInfo"]/*'/>
    public class ClientInfo
    {
        public string Ip { get; set; }
        public string LastCommand { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }
}