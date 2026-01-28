using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AccountDepositCommand"]/*'/>
    public class AccountDepositCommand : ICommand, IAccountInteractionCommand
    {
        public string Key => "AD";
        public AccountDAO DAO { get; }

        public AccountDepositCommand(AccountDAO dao)
        {
            DAO = dao;
        }
        
        /// <summary>
        /// Executes the Account Deposit command (AD).
        /// </summary>
        /// <param name="arguments">
        /// Command arguments in format ACCOUNT/IP AMOUNT,
        /// where ACCOUNT is the account number, IP is the target bank address and AMOUNT is the value to be deposited.
        /// </param>
        /// <returns>
        /// Returns "AD" if the deposit is performed on the local bank,
        /// otherwise returns the response from the remote bank node.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the arguments are in an invalid format, the account number
        /// cannot be parsed, or the amount is not a valid number.
        /// </exception>
        public string Execute(string arguments)
        {
            string[] argumentParts = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (argumentParts.Length != 2)
            {
                throw new FormatException("Invalid format. Usage: AD <account>/<ip> <amount>");
            }

            string[] accountParts = argumentParts[0].Split('/');

            if (accountParts.Length != 2)
            {
                throw new FormatException("Invalid account format.");
            }

            if (!int.TryParse(accountParts[0], out int accountNumber))
            {
                throw new FormatException("Invalid account number.");
            }
            string targetIp = accountParts[1];

            if (!long.TryParse(argumentParts[1], out long amount))
            {
                throw new FormatException("Invalid amount.");
            }

            string? localIp = IPAddressObtainer.GetLocalIPv4Address();
            if (targetIp == localIp)
            {
                DAO.Deposit(accountNumber, amount);
                return "AD";
            }
            else
            {
                string originalCommand = $"AD {accountNumber}/{targetIp} {amount}";
                return BankProxyClient.Forward(targetIp, originalCommand);
            }
        }
    }
}
