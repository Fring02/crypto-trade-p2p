using Domain.Models.Base;

namespace Domain.Models;
public class User : BaseEntity<Guid>
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Email { get; set; }
    public byte[] Hash { get; set; }
    public byte[] Salt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshExpires { get; set; }
    public ICollection<RequisiteDetails> Requisites { get; set; }
    public string Role { get; set; }
}