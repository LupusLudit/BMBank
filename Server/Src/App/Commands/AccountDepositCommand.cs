using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    public class AccountDepositCommand : IAsyncCommand, IAccountInteractionCommand
    {
        public string Key => "AD";
        public string Execute(string arguments)
        {
            throw new NotImplementedException();
        }

        public AccountDAO DAO { get; }

        public AccountDepositCommand(AccountDAO dao) => DAO = dao;

        public async Task<string> ExecuteAsync(string arguments, CancellationToken token)
        {
            string[] argumentParts = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (argumentParts.Length != 2)
                throw new FormatException("Invalid format. Usage: AD <account>/<ip> <amount>");

            string[] accountParts = argumentParts[0].Split('/');
            if (accountParts.Length != 2)
                throw new FormatException("Invalid account format.");

            if (!int.TryParse(accountParts[0], out int accountNumber))
                throw new FormatException("Invalid account number.");

            string targetIp = accountParts[1];
            if (!long.TryParse(argumentParts[1], out long amount))
                throw new FormatException("Invalid amount.");

            string localIp = IPAddressObtainer.GetLocalIPv4Address();
            if (targetIp == localIp)
            {
                DAO.Deposit(accountNumber, amount);
                return "AD";
            }
            else
            {
                string originalCommand = $"AD {accountNumber}/{targetIp} {amount}";
                return await BankProxyClient.ForwardAsync(targetIp, originalCommand, token)
                       ?? "ER No bank found";
            }
        }
    }
}