namespace Domain.Settings;

public record ConnectionStrings
{
    public string DefaultConnection { get; set; }
    public string RequisitesUrl { get; set; }
}