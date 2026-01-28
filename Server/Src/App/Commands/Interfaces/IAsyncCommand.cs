namespace BMBank.Src.App.Commands.Interfaces;

public interface IAsyncCommand : ICommand
{
    Task<string> ExecuteAsync(string arguments, CancellationToken token);
}