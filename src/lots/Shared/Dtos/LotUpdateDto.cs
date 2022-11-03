using System.Text.Json.Serialization;
using Shared.Interfaces.Dtos;

namespace Shared.Dtos;

public record LotUpdateDto : IDto<long>
{
    public string? LotType { get; set; }
    public double? Price { get; set; }
    public double? Supply { get; set; }
    public double? MinLimit { get; set; }
    public double? MaxLimit { get; set; }
    public string? FiatType { get; set; }
    public string? CryptoType { get; set; }
    public long? RequisiteId { get; set; }
    [JsonIgnore]
    public long Id { get; set; }
}