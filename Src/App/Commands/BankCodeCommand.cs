using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    public class BankCodeCommand : ICommand
    {
        public string Key => "BC";

        public string Execute(string arguments)
        {
            if (!string.IsNullOrEmpty(arguments))
            {
                throw new ArgumentException("BC command does not take any arguments.");
            }

            string? ipAddress = IPAddressObtainer.GetLocalIPv4Address();

            if (ipAddress == null)
            {
                throw new InvalidOperationException("No IPv4 address found for the host.");
            }

            return $"BC {ipAddress}";
        }
    }
}
