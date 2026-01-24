using BMBank.Src.DatabaseInteraction.Core.DAO;

namespace BMBank.Src.App.Commands.Interfaces
{
    public interface IAccountInteractionCommand
    {
        public AccountDAO DAO { get; }
    }
}
