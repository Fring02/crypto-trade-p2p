namespace Application.Commands.Ethereum.Wallets;

public record CreateEthereumWalletCommand(string Email, string Password)
{
    public string Email { get; set; } = Email;
    public string Password { get; set; } = Password;
}