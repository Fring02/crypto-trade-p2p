namespace Shared.Interfaces.Dtos;
public interface IUpdateDto<TId>
{
    TId Id { get; set; }
}