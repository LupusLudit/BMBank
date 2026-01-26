namespace BMBank.Src.Network;

public static class BankNetworkExplorer
{
    public static IEnumerable<string> DiscoverBanks()
    {
        string prefix = NetworkConfig.NetworkPrefix;
        string[] parts = prefix.Split('.');

        if (parts.Length < 1 || parts.Length > 3)
        {
            throw new InvalidOperationException("Invalid BankNetworkPrefix format.");
        }
        
        int missingOctets = 4 - parts.Length;
        
        if (missingOctets == 1)
        {
            for (int i = 1; i < 255; i++)
            {
                yield return $"{prefix}.{i}";
            }
        }
        else if (missingOctets == 2)
        {
            for (int i = 1; i < 255; i++)
            {
                for (int j = 1; j < 255; j++)
                {
                    yield return $"{prefix}.{i}.{j}";
                }
            }
        }
        else if (missingOctets == 3)
        {
            for (int i = 1; i < 255; i++)
            {
                for (int j = 1; j < 255; j++)
                {
                    for (int k = 1; k < 255; k++)
                    {
                        yield return $"{prefix}.{i}.{j}.{k}";
                    }
                }
            }
        }
    }

    public record BankInfo(string Ip, long Money, int Clients);
}