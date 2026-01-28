using System.Net;
using System.Net.Sockets;

namespace BMBank.Src.Network
{
    /// <include file='../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="ClientSession"]/*'/>
    public static class IPAddressObtainer
    {

        /// <summary>
        /// Obtains the local IPv4 address.
        /// </summary>
        /// <returns>
        /// The IP address found; or null if no address was found.
        /// </returns>
        public static string? GetLocalIPv4Address()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            var ipAddress = hostEntry.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

            return ipAddress?.ToString();
        }
    }
}
