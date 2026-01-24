using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;
namespace BMBank.Src.App.Commands
{
    public class AccountDepositCommand: ICommand, IAccountInteractionCommand
    {
        public string Key => "AD";

        public AccountDAO DAO { get; }

        public AccountDepositCommand(AccountDAO dao)
        {
            DAO = dao;
        }

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
            if (targetIp != localIp)
            { 
                throw new InvalidOperationException("Invalid ip address provided.");
            }

            DAO.Deposit(accountNumber, amount);

            return "AD";
        }
    }
}
