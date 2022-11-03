namespace Shared.Dtos;

public record RequisiteItemDto
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
    public string CreditCardNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string BankName { get; set; }
}