namespace AuthService.Models;

public class User
{
    public Guid Id { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public byte[] Hash { get; set; }
    public byte[] Salt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshExpires { get; set; }
    public string Role { get; set; }
}