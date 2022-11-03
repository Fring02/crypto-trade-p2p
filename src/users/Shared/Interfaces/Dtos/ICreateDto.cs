namespace Shared.Interfaces.Dtos;

public interface ICreateDto<TId>
{
    TId Id { get; set; }
}