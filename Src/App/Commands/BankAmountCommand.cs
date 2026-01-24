using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;

namespace BMBank.Src.App.Commands
{
    public class BankAmountCommand : ICommand, IAccountInteractionCommand
    {
        public string Key => "BA";
        public AccountDAO DAO { get; }

        public BankAmountCommand(AccountDAO dao)
        {
            DAO = dao;
        }

        public string Execute(string arguments)
        {
            if (!string.IsNullOrWhiteSpace(arguments))
            {
                throw new ArgumentException("BA command takes no arguments.");
            }

            long total = DAO.GetTotalAmount();

            return $"BA {total}";
        }
    }
}
