namespace Shared.Dtos;

public record RequisiteViewDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string CreditCardNumber { get; set; }
    public string PhoneNumber { get; set; }
    public string BankName { get; set; }
}