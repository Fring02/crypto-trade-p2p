namespace Domain.Settings;

public record SessionSettings
{
    public int ExpirationInMinutes { get; set; }
    public string SessionStreamName { get; set; }
    public string TransferUrl { get; set; }
    public string WalletsUrl { get; set; }
}