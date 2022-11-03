using Domain.Models.Base;

namespace Domain.Models;

public class RequisiteDetails : BaseEntity<long>
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CreditCardNumber { get; set; }
    public string? BankName { get; set; }
}