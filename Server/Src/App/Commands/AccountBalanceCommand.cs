using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AccountBalanceCommand"]/*'/>
    public class AccountBalanceCommand : ICommand, IAccountInteractionCommand
    {
        public string Key => "AB";
        public AccountDAO DAO { get; }
        public AccountBalanceCommand(AccountDAO dao)
        {
            DAO = dao;
        }

        /// <summary>
        /// Executes the Account Balance command (AB).
        /// </summary>
        /// <param name="arguments">
        /// Command arguments in format ACCOUNT/IP,
        /// where ACCOUNT is the account number and IP is the target bank address.
        /// </param>
        /// <returns>
        /// Returns the account balance in format "AB BALANCE" if the account is local,
        /// otherwise returns the response from the remote bank node.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the arguments are in an invalid format or the account number cannot be parsed.
        /// </exception>
        public string Execute(string arguments)
        {
            string[] accountParts = arguments.Trim().Split('/');
            if (accountParts.Length != 2)
            {
                throw new FormatException("Invalid format. Usage: AB <account>/<ip>");
            }

            if (!int.TryParse(accountParts[0], out int accountNumber))
            {
                throw new FormatException("Invalid account number.");
            }
            string targetIp = accountParts[1];

            string? localIp = IPAddressObtainer.GetLocalIPv4Address();

            if (targetIp == localIp)
            {
                long balance = DAO.GetBalance(accountNumber);
                return $"AB {balance}";
            }
            else
            {
                string originalCommand = $"AB {accountNumber}/{targetIp}";
                return BankProxyClient.Forward(targetIp, originalCommand);
            }
        }
    }
}
