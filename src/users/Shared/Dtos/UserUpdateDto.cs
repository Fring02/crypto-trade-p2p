using Shared.Interfaces.Dtos;

namespace Shared.Dtos;

public record UserUpdateDto : IUpdateDto<Guid>
{
    public Guid Id { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}