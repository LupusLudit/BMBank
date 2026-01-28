using BMBank.Src.App.Commands;
using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using Microsoft.Data.SqlClient;

namespace BMBank.Src.App
{
    /// <include file='../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="CommandHandler"]/*'/>
    public class CommandHandler
    {
        private Dictionary<string, ICommand> commands;
        private ClientCommandLogDAO logDao;

        /// <summary>
        /// Initializes the command handler with the given database connection.
        /// Sets up all available commands.
        /// </summary>
        /// <param name="connection">Active SQL database connection.</param>
        public CommandHandler(SqlConnection connection)
        {
            AccountDAO dao = new AccountDAO(connection);
            logDao = new ClientCommandLogDAO(connection);

            List<ICommand> commandList = new List<ICommand>
            {
                new BankCodeCommand(),
                new AccountCreateCommand(dao),
                new AccountDepositCommand(dao),
                new AccountWithdrawCommand(dao),
                new AccountBalanceCommand(dao),
                new AccountRemoveCommand(dao),
                new BankAmountCommand(dao),
                new BankClientCountCommand(dao),
                new RobberyPlanCommand()
            };

            commands = commandList.ToDictionary(command => command.Key, command => command);
        }

        /// <summary>
        /// Parses the input string, extracts the command key and arguments,
        /// and executes the corresponding command.
        /// </summary>
        /// <param name="input">The raw command string input by the client.</param>
        /// <returns>
        /// The result of the command execution as a string.
        /// Returns an empty string if the input is empty or whitespace.
        /// </returns>
        public string ProcessCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            input = input.Trim();

            if (input.Length < 2)
            {
                throw new FormatException("Input too short");
            }

            string commandKey = input.Substring(0, 2).ToUpper();
            string arguments = input.Length > 2 ? input.Substring(2).Trim() : string.Empty;

            if (commands.TryGetValue(commandKey, out ICommand? command))
            {
                return command.Execute(arguments);
            }
            else
            {
                throw new InvalidOperationException($"Unknown command: {commandKey}");
            }
        }
    }
}

