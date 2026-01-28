using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
<<<<<<< HEAD
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AccountWithdrawCommand"]/*'/>
    public class AccountWithdrawCommand : ICommand, IAccountInteractionCommand
    {
        public string Key => "AW";
        public AccountDAO DAO { get; }

        public AccountWithdrawCommand(AccountDAO dao)
        {
            DAO = dao;
        }
        
        /// <summary>
        /// Executes the Account Withdraw command (AW).
        /// </summary>
        /// <param name="arguments">
        /// Command arguments in format ACCOUNT/IP AMOUNT,
        /// where ACCOUNT is the account number, IP is the target bank node,
        /// and AMOUNT is the amount to withdraw.
        /// </param>
        /// <returns>
        /// Returns "AW" if the withdrawal was performed locally,
        /// or forwards the command to a remote bank node if the IP differs.
        /// </returns>
        /// <exception cref="FormatException">
        /// Thrown when the arguments are invalid:
        /// - Wrong format
        /// - Account number cannot be parsed
        /// - Amount is not a valid long
        /// </exception>

=======
    public class AccountWithdrawCommand : IAsyncCommand, IAccountInteractionCommand
    {
        public string Key => "AW";
>>>>>>> 48d90d1ccacc2ec931ab9ab9039ef5a2accccd61
        public string Execute(string arguments)
        {
            throw new NotImplementedException();
        }

        public AccountDAO DAO { get; }

        public AccountWithdrawCommand(AccountDAO dao) => DAO = dao;

        public async Task<string> ExecuteAsync(string arguments, CancellationToken token)
        {
            string[] parts = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
                throw new FormatException("Invalid format. Usage: AW <account>/<ip> <amount>");

            string[] accountParts = parts[0].Split('/');
            if (accountParts.Length != 2)
                throw new FormatException("Invalid account format.");

            if (!int.TryParse(accountParts[0], out int accountNumber))
                throw new FormatException("Invalid account number.");

            string targetIp = accountParts[1];
            if (!long.TryParse(parts[1], out long amount))
                throw new FormatException("Invalid amount.");

            string localIp = IPAddressObtainer.GetLocalIPv4Address();
            if (targetIp == localIp)
            {
                DAO.Withdraw(accountNumber, amount);
                return "AW";
            }
            else
            {
                string originalCommand = $"AW {accountNumber}/{targetIp} {amount}";
                return await BankProxyClient.ForwardAsync(targetIp, originalCommand, token)
                       ?? "ER No bank found";
            }
        }
    }
}