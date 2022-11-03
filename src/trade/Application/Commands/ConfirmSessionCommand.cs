using MediatR;
namespace Application.Commands;

public record ConfirmSessionCommand(long SessionId) : IRequest;