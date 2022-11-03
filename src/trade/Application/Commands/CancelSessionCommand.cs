using MediatR;

namespace Application.Commands;

public record CancelSessionCommand(long SessionId) : IRequest;