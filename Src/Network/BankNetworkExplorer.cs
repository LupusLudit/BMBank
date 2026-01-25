namespace BMBank.Src.Network;

public static class BankNetworkExplorer
{
    public static IEnumerable<string> DiscoverBanks()
    {
        for (int i = 1; i < 255; i++)
        {
            yield return $"10.1.2.{i}";
        }
    }

    public record BankInfo(string Ip, long Money, int Clients);
}