namespace BMBank.Src.DatabaseInteraction.Core.DBEntities;

public class ClientCommandLog : IDBEntity
{
    public int ID { get; }
    public string ClientIp { get; }
    public string Command { get; }
    public string Arguments { get; }
    public DateTime ExecutedAt { get; }
    public string ResultStatus { get; }
}