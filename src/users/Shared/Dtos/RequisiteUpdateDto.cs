using Shared.Interfaces.Dtos;

namespace Shared.Dtos;

public record RequisiteUpdateDto : IUpdateDto<long>
{
    public long Id { get; set; }
    public string? PhoneNumber { get; set; }
    public string? CreditCardNumber { get; set; }
    public string? BankName { get; set; }
}