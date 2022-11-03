using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Base;

public class BaseEntity<TId> where TId : struct
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public TId Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? ModifiedAt { get; set; }
}