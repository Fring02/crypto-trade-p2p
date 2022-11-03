using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Shared.Interfaces.Dtos;

namespace Shared.Dtos;

public record RequisiteCreateDto : ICreateDto<long>
{
    [Required]
    public Guid UserId { get; set; }
    [RegularExpression(@"^\+?77([0124567][0-8]\d{7})$")]
    public string PhoneNumber { get; set; }
    [CreditCard]
    public string CreditCardNumber { get; set; }
    [Required]
    public string BankName { get; set; }
    [JsonIgnore]
    public long Id { get; set; }
}