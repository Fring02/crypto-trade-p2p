using System.Text.Json.Serialization;
using MediatR;

namespace Application.Commands;

public record CompleteSessionCommand : IRequest
{
    [JsonIgnore]
    public long SessionId { get; set; }
    public string RecipientId { get; set; }
    public decimal Amount { get; set; }
    [JsonIgnore]
    public string AccessToken { get; set; }
    [JsonIgnore]
    public string RefreshToken { get; set; }
}