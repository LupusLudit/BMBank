using BMBank.Src.App.Commands;
using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BookOrg.Src.Logic.Connection;
using Microsoft.Data.SqlClient;

namespace BMBank.Src.App
{
    public class CommandHandler
    {
        private Dictionary<string, ICommand> commands;

        public CommandHandler(SqlConnection connection)
        {
            AccountDAO dao = new AccountDAO(connection);

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

