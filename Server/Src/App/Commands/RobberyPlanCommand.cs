using System.Collections.Concurrent;
using BMBank.Src.App.Commands.Interfaces;
using BMBank.Src.Network;

namespace BMBank.Src.App.Commands
{
    /// <include file='../../../Docs/ClassDocumentation.xml' path='ClassDocumentation/ClassMembers[@name="RobberyPlanCommand"]/*'/>
    public class RobberyPlanCommand : IAsyncCommand
    {
        public string Key => "RP";
        public string Execute(string arguments)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ExecuteAsync(string arguments, CancellationToken token)
        {
            if (!long.TryParse(arguments.Trim(), out long target))
                throw new FormatException("ER Invalid format. Usage: RP <number>");
            
            var scanner = new OnDemandNetworkScanner(NetworkConfig.PortFrom, NetworkConfig.PortTo, TimeSpan.FromMilliseconds(300), maxParallel: 300);
            
            var ips = BankNetworkExplorer.DiscoverBanks();
            var banks = await scanner.ScanAsync(ips, token);
            
            var usableBanks = banks.Where(b => b.Total > 0).ToList();

            if (!usableBanks.Any())
            {
                return "RP No banks available in the network";
            }
            
            var plan = usableBanks.OrderBy(b => b.Clients).ThenByDescending(b => b.Total).ToList();

            long sum = 0;
            int harmed = 0;
            var selected = new List<OnDemandNetworkScanner.DiscoveredBank>();

            foreach (var bank in plan)
            {
                if (sum >= target)
                    break;
                
                selected.Add(bank);
                sum += bank.Total;
                harmed += bank.Clients;
            }

            if (sum < target)
            {
                return $"RP Could not reach {target}, even when rob every bank";
            }
            
            string bankList = string.Join(" a ", selected.Select(b => b.Ip));
            
            return
                $"RP For reaching {target} it is needed to rob banks {bankList} " +
                $"and {harmed} clients will be harmed.";
        }
    }
}