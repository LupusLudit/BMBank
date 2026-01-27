using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    public class BankCodeCommand : ICommand
    {
        public string Key => "BC";

        /// <summary>
        /// Executes the Bank Code command (BC).
        /// </summary>
        /// <param name="arguments">
        /// BC command does not take any arguments.
        /// </param>
        /// <returns>
        /// Returns the local IP address of the host as "BC IP_ADDRESS".
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if any arguments are provided to the BC command.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no local IP address could be determined for the host.
        /// </exception>
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
