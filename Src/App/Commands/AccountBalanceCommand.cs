using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;
namespace BMBank.Src.App.Commands
{
    public class AccountBalanceCommand : ICommand, IAccountInteractionCommand
    {
        public string Key => "AB";
        public AccountDAO DAO { get; }
        public AccountBalanceCommand(AccountDAO dao)
        {
            DAO = dao;
        }

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

            if (targetIp != localIp)
            {
                throw new InvalidOperationException("Invalid ip address provided.");
            }

            long balance = DAO.GetBalance(accountNumber);

            return $"AB {balance}";
        }
    }
}
