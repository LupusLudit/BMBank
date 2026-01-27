using System.Net;
using System.Net.Sockets;

namespace BMBank.Src.Network
{
    /// <summary>
    /// Gets the first available local IPv4 address of the host machine.
    /// </summary>
    /// <returns>
    /// A string representing the local IPv4 address, or null if none is found.
    /// </returns>
    public static class IPAddressObtainer
    {
        public static string? GetLocalIPv4Address()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
            var ipAddress = hostEntry.AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

            return ipAddress?.ToString();
        }
    }
}
