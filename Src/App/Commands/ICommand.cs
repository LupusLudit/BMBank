namespace BMBank.Src.App.Commands
{
    public interface ICommand
    {
        public string Key { get; }
        public string Execute(string arguments);
    }
}
