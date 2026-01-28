using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="AccountBalanceCommand"]/*'/>
    public class AccountBalanceCommand : IAsyncCommand, IAccountInteractionCommand
    {
        public string Key => "AB";
        public string Execute(string arguments)
        {
            throw new NotImplementedException();
        }

        public AccountDAO DAO { get; }

        public AccountBalanceCommand(AccountDAO dao) => DAO = dao;

        public async Task<string> ExecuteAsync(string arguments, CancellationToken token)
        {
            string[] parts = arguments.Trim().Split('/');
            if (parts.Length != 2)
            {
                throw new FormatException("Invalid format. Usage: AB <account>/<ip>");
            }

            if (!int.TryParse(parts[0], out int accountNumber))
            {
                throw new FormatException("Invalid account number.");
            }

            string targetIp = parts[1];
            string localIp = IPAddressObtainer.GetLocalIPv4Address();

            if (targetIp == localIp)
            {
                long balance = DAO.GetBalance(accountNumber);
                return $"AB {balance}";
            }
            else
            {
                string originalCommand = $"AB {accountNumber}/{targetIp}";
                return await BankProxyClient.ForwardAsync(targetIp, originalCommand, token) ?? "ER No bank found";
            }
        }
    }
}