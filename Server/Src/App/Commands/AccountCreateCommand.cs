using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.DatabaseInteraction.Core.DAO;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    public class AccountCreateCommand : ICommand, IAccountInteractionCommand
    {
        public string Key => "AC";
        public AccountDAO DAO { get; }

        public AccountCreateCommand(AccountDAO dao)
        {
            DAO = dao;
        }

        public string Execute(string arguments)
        {
            string? localIp = IPAddressObtainer.GetLocalIPv4Address();
            if (localIp == null)
            {
                throw new InvalidOperationException("Cannot determine local IP.");
            }

            int newAccountNumber = DAO.CreateAccount();

            return $"AC {newAccountNumber}/{localIp}";
        }
    }
}
