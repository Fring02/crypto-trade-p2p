namespace Domain.Models.Base;
public interface IWallet<TId> 
{
    public TId Id { get; set; }
    public string Email { get; set; }
    public DateTime? UnlockDate { get; set; }
}