namespace Shared.Dtos;

public record UserItemDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}