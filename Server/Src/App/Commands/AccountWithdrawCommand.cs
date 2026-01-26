using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    public class AccountWithdrawCommand : ICommand, IAccountInteractionCommand
    {
        public string Key => "AW";
        public AccountDAO DAO { get; }


        public AccountWithdrawCommand(AccountDAO dao)
        {
            DAO = dao;
        }


        public string Execute(string arguments)
        {
            string[] argumentsParts = arguments.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (argumentsParts.Length != 2)
            {
                throw new FormatException("Invalid format. Usage: AW <account>/<ip> <amount>");
            }

            string[] accountParts = argumentsParts[0].Split('/');
            if (accountParts.Length != 2)
            {
                throw new FormatException("Invalid account format.");
            }

            if (!int.TryParse(accountParts[0], out int accountNumber))
            {
                throw new FormatException("Invalid account number.");
            }
            string targetIp = accountParts[1];

            if (!long.TryParse(argumentsParts[1], out long amount))
            {
                throw new FormatException("Invalid amount.");
            }

            string? localIp = IPAddressObtainer.GetLocalIPv4Address();
            if (targetIp == localIp)
            {
                DAO.Withdraw(accountNumber, amount);
                return "AW";
            }
            else
            {
                string originalCommand = $"AW {accountNumber}/{targetIp} {amount}";
                return BankProxyClient.Forward(targetIp, originalCommand);
            }
        }
    }
}
