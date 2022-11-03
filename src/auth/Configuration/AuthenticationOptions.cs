namespace AuthService.Configuration;

public record AuthenticationOptions
{
    public int AccessLifetimeInMinutes { get; set; }
    public int RefreshLifetimeInDays { get; set; }
    public string SecretKey { get; set; }
}