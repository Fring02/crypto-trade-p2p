namespace Domain.Settings;

public record EmailSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Security { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Link { get; set; }
}