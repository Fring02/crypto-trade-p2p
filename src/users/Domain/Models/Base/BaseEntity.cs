using System.ComponentModel.DataAnnotations;

namespace Domain.Models.Base;

public class BaseEntity<TId> where TId : struct
{
    [Key]
    public TId Id { get; set; }
}