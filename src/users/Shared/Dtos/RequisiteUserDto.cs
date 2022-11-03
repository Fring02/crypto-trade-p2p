namespace Shared.Dtos;

public record RequisiteUserDto
{
    public long Id { get; set; }
    public Guid UserId { get; set; }
}