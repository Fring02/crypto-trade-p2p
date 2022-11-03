using Domain.Enums;
using Domain.Models.Base;
namespace Domain.Models;
public class Session : BaseEntity<long>
{
    public DateTime StartsAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public SessionDetails Details { get; set; }
    public SessionStatus SessionStatus { get; set; }
    public long LotId { get; set; }
}