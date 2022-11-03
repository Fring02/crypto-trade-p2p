namespace Shared.Dtos;

public record LotOwnerDto
{
    public long Id { get; set; }
    public string OwnerEmail { get; set; }
}