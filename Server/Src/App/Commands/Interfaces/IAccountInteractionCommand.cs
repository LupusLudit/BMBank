using BMBank.Src.DatabaseInteraction.Core.DAO;

namespace BMBank.Src.App.Commands.Interfaces
{
    /// <include file='../../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="IAccountInteractionCommand"]/*'/>
    public interface IAccountInteractionCommand
    {
        public AccountDAO DAO { get; }
    }
}
