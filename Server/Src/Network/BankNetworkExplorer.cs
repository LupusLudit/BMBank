namespace BMBank.Src.Network;

public static class BankNetworkExplorer
{
    /// <summary>
    /// Discovers all possible bank IP addresses based on the configured network prefix.
    /// </summary>
    /// <returns>
    /// An enumerable of strings representing possible bank IP addresses.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the network prefix from is not in a valid format (1–3 octets).
    /// </exception>
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

    /// <summary>
    /// Represents information about a bank node.
    /// </summary>
    /// <param name="Ip">The IP address of the bank.</param>
    /// <param name="Money">The total money held by the bank.</param>
    /// <param name="Clients">The number of clients in the bank.</param>
    public record BankInfo(string Ip, long Money, int Clients);
}