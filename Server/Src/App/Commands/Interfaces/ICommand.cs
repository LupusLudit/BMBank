namespace BMBank.Src.App.Commands.Interfaces
{
    public interface ICommand
    {
        public string Key { get; }
        public string Execute(string arguments);
    }
}
