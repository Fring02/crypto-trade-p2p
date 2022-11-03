namespace Domain.Models;

public record SessionMessage
{
    public long SessionId { get; set; }
    public long LotId { get; set; }
    public string Text { get; set; }
    public string RecipientEmail { get; set; }
    public override string ToString() => $"Session information: Id = {SessionId}, Lot = {LotId}, Recipient = {RecipientEmail}, Text = {Text}";
}