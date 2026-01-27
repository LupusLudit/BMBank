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

        /// <summary>
        /// Executes the Bank Client Count command (BN).
        /// </summary>
        /// <param name="arguments">
        /// BN command takes no arguments.
        /// </param>
        /// <returns>
        /// Returns the number of clients connected to the bank as "BN COUNT".
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown if any arguments are provided to the BN command.
        /// </exception>
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
