using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;

namespace BMBank.Src.App.Commands
{
    public class BankClientCountCommand : ICommand, IAccountInteractionCommand
    {
        public string Key => "BN";
        public AccountDAO DAO { get; }

        public BankClientCountCommand(AccountDAO dao)
        {
            DAO = dao;
        }

        public string Execute(string arguments)
        {
            if (!string.IsNullOrWhiteSpace(arguments))
            {
                throw new ArgumentException("BN command takes no arguments.");
            }

            int count = DAO.GetClientCount();

            return $"BN {count}";
        }
    }
}
