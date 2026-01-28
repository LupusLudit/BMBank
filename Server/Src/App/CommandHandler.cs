using BMBank.Src.App.Commands;
using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.Common;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using Microsoft.Data.SqlClient;

namespace BMBank.Src.App;

public class CommandHandler
{
    private readonly Dictionary<string, ICommand> commands;

    public CommandHandler(SqlConnection connection)
    {
        AccountDAO dao = new(connection);

        var commandList = new List<ICommand>
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

        commands = commandList.ToDictionary(c => c.Key, c => c);
    }

    /// <summary>
    /// Processes a command asynchronously.
    /// Supports both synchronous (local) and asynchronous (remote) commands.
    /// </summary>
    /// <param name="input">The raw command string.</param>
    /// <param name="token">Cancellation token to allow timeouts or cancellation.</param>
    /// <returns>The result of the command.</returns>
    public async Task<string> ProcessCommandAsync(string input, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = input.Trim();
        if (input.Length < 2)
            throw new FormatException("Input too short");

        string key = input.Substring(0, 2).ToUpper();
        string args = input.Length > 2 ? input.Substring(2).Trim() : "";

        if (!commands.TryGetValue(key, out ICommand? command))
            throw new InvalidOperationException($"Unknown command: {key}");
        
        if (command is IAsyncCommand asyncCmd)
        {
            return await SafeExecutor.ExecuteAsync(() => asyncCmd.ExecuteAsync(args, token));
        }
        else
        {
            return SafeExecutor.Execute(() => command.Execute(args));
        }
    }
}
