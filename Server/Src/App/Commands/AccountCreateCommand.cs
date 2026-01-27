using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    public class AccountCreateCommand : ICommand, IAccountInteractionCommand
    {
        public string Key => "AC";
        public AccountDAO DAO { get; }

        public AccountCreateCommand(AccountDAO dao)
        {
            DAO = dao;
        }
        
        /// <summary>
        /// Executes the Account Create command (AC).
        /// </summary>
        /// <param name="arguments">
        /// This command does not accept any arguments.
        /// </param>
        /// <returns>
        /// Returns the newly created account identifier in format "AC ACCOUNT/IP",
        /// where ACCOUNT is the new account number and IP is the local bank node address.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the local IP address cannot be determined.
        /// </exception>
        public string Execute(string arguments)
        {
            string? localIp = IPAddressObtainer.GetLocalIPv4Address();
            if (localIp == null)
            {
                throw new InvalidOperationException("Cannot determine local IP.");
            }

            int newAccountNumber = DAO.CreateAccount();

            return $"AC {newAccountNumber}/{localIp}";
        }
    }
}
