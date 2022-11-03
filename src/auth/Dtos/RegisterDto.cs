using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using AuthService.Configuration;

namespace AuthService.Dtos;
public record RegisterDto
{
    [Required]
    public string Firstname { get; set; }
    [Required]
    public string Lastname { get; set; }
    [Required, EmailAddress]
    public string Email { get; set; }
    [Required, MinLength(8)]
    public string Password { get; set; }
    [Required, Compare("Password")]
    public string RepeatPassword { get; set; }
    public string? PrivateKey { get; set; }
    [JsonIgnore]
    public string Role { get; set; } = Roles.USER;
}