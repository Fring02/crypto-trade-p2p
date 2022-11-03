using System.ComponentModel.DataAnnotations;
namespace AuthService.Dtos;
public record LoginDto([Required, EmailAddress] string Email, [Required] string Password);