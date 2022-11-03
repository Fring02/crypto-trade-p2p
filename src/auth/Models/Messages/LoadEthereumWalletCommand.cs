// ReSharper disable once CheckNamespace
namespace Application.Commands.Ethereum.Wallets;

public record LoadEthereumWalletCommand : CreateEthereumWalletCommand
{
    public LoadEthereumWalletCommand(string email, string password, string privateKey) : base(email, password)
    {
        Email = email;
        Password = password;
        PrivateKey = privateKey;
    }
    public string PrivateKey { get; set; }
}