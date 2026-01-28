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
        
        /// <summary>
        /// Executes the Bank Amount command (BA).
        /// </summary>
        /// <param name="arguments">
        /// BA command takes no arguments.
        /// </param>
        /// <returns>
        /// Returns the total amount of money in the bank as "BA TOTAL".
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if any arguments are provided to the BA command.
        /// </exception>
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
