using System.Net;
using System.Net.Sockets;

namespace BMBank.Src.Network
{
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
