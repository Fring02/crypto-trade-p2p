namespace Data.Configuration;

public record MongoSettings {
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; } 
    public string EthereumWalletsCollection { get; set; }
    public string EthereumP2PWalletsCollection { get; set; }
}