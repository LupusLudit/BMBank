namespace BMBank.Src.App.Commands.Interfaces
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="ICommand"]/*'/>

    public interface ICommand
    {
        public string Key { get; }
        public string Execute(string arguments);
    }
}
