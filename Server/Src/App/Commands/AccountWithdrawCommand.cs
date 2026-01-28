using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AccountWithdrawCommand"]/*'/>
    public class AccountWithdrawCommand : IAsyncCommand, IAccountInteractionCommand
    {
        public string Key => "AW";
        public string Execute(string arguments)
        {
            throw new NotImplementedException();
        }

        public AccountDAO DAO { get; }

        public AccountWithdrawCommand(AccountDAO dao)
        {
            DAO = dao;
        }

        /// <summary>
        /// Executes the account withdraw command asynchronously.
        /// </summary>
        /// <param name="arguments">
        /// Command arguments in format: AW ACCOUNT/IP AMOUNT.
        /// </param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>
        /// A confirmation string or an error message.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the argument format, account number, or amount is invalid.
        /// </exception>
        public async Task<string> ExecuteAsync(string arguments, CancellationToken token)
        {
            string[] parts = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                throw new FormatException("Invalid format. Usage: AW <account>/<ip> <amount>");
            }

            string[] accountParts = parts[0].Split('/');
            if (accountParts.Length != 2)
            {
                throw new FormatException("Invalid account format.");
            }

            if (!int.TryParse(accountParts[0], out int accountNumber))
            {
                throw new FormatException("Invalid account number.");
            }

            string targetIp = accountParts[1];
            if (!long.TryParse(parts[1], out long amount))
            {
                throw new FormatException("Invalid amount.");
            }

            string localIp = IPAddressObtainer.GetLocalIPv4Address();
            if (targetIp == localIp)
            {
                DAO.Withdraw(accountNumber, amount);
                return "AW";
            }
            else
            {
                string originalCommand = $"AW {accountNumber}/{targetIp} {amount}";
                var result = await BankProxyClient.ForwardAsync(targetIp, originalCommand, token);

                if (result == null)
                {
                    return "ER No bank found";
                }

                return result;
            }
        }
    }
}