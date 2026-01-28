using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AccountRemoveCommand"]/*'/>
    public class AccountRemoveCommand : ICommand, IAccountInteractionCommand
    {
        public string Key => "AR";
        public AccountDAO DAO { get; }

        public AccountRemoveCommand(AccountDAO dao)
        {
            DAO = dao;
        }

        /// <summary>
        /// Executes the Account Remove command (AR).
        /// </summary>
        /// <param name="arguments">
        /// Command arguments in format ACCOUNT/IP,
        /// where ACCOUNT is the account number and IP is the target bank address.
        /// </param>
        /// <returns>
        /// Returns "AR" when the account is successfully removed from the local bank.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the arguments are in an invalid format or the account number cannot be parsed.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the provided IP address does not match the local bank node.
        /// </exception>
        public string Execute(string arguments)
        {
            string[] accountParts = arguments.Trim().Split('/');
            if (accountParts.Length != 2)
            {
                throw new FormatException("Invalid format. Usage: AR <account>/<ip>");
            }

            if (!int.TryParse(accountParts[0], out int accountNumber))
            {
                throw new FormatException("Invalid account number.");
            }
            string targetIp = accountParts[1];

            string? localIp = IPAddressObtainer.GetLocalIPv4Address();

            if (targetIp != localIp)
            {
                throw new InvalidOperationException("Invalid ip address provided.");
            }

            DAO.RemoveAccount(accountNumber);

            return "AR";
        }
    }
}
