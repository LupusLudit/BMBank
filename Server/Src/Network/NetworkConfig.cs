using System.Configuration;

namespace BMBank.Src.Network
{
    /// <include file='../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="NetworkConfig"]/*'/>
    public static class NetworkConfig
    {
        public static int PortFrom => int.Parse(ConfigurationManager.AppSettings["BankPortFrom"]!);

        public static int PortTo => int.Parse(ConfigurationManager.AppSettings["BankPortTo"]!);

        public static string NetworkPrefix => ConfigurationManager.AppSettings["BankNetworkPrefix"]!;
    }
}