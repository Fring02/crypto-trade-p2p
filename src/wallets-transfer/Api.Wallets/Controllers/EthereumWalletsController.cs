using System.IdentityModel.Tokens.Jwt;
using Application.Commands.Ethereum.Wallets.Buy;
using Application.Commands.Ethereum.Wallets.Sell;
using Application.Queries.Ethereum;
using Application.Queries.Ethereum.Wallets;
using Application.Queries.Ethereum.Wallets.P2P;
using Application.Responses.Ethereum.Wallets;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Wallets.Api.Controllers;
/// <summary>
/// Controller for getting ETH wallets and setting amount to sell and buy.
/// </summary>
[ApiController]
[Route("api/v1/wallets/eth")]
public class EthereumWalletsController : ControllerBase
{
    private readonly IMediator _mediator;
    public EthereumWalletsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id}/privateKey")]
    public async Task<IActionResult> GetWalletPrivateKey(string id, CancellationToken token)
    {
        if (!IsParsable(id)) return BadRequest("Wallet id is invalid");
        var accessToken = HttpContext.Request.Cookies["jwt-access"]?.Split(' ').Last();
        if (!TryGetEmailFromToken(accessToken, out var email)) return Unauthorized();
        var privateKey = await _mediator.Send(new GetPrivateKeyQuery(id, email), token); 
        return Ok(privateKey);
    }

    private static bool TryGetEmailFromToken(string? token, out string email)
    {
        if (string.IsNullOrEmpty(token)) { email = ""; return false; }
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) { email = ""; return false; }
        var user = handler.ReadJwtToken(token);
        var matchEmail = user.Claims.FirstOrDefault(x => x.Type == "user_id")?.Value;
        if (string.IsNullOrEmpty(matchEmail)) { email = ""; return false; }
        email = matchEmail;
        return true;
    }

    /// <summary>
    /// Returns information about user's main Ethereum wallet: Ethereum account's address, private key, and balance in Ether
    /// </summary>
    /// <param name="id">Wallet's id in ObjectID format</param>
    /// <param name="email">User email</param>
    /// <param name="token"></param>
    /// <returns>Wallet's information: address, private key, and balance in Ether</returns>
    /// <remarks>
    /// Sample request:
    ///     GET /eth/6277d227108472b96eee5e56
    /// 
    /// Sample response:
    /// 
    ///     {
    ///         "balance": 95.58789490,
    ///         "address": "0xD2BE74365557b91070405d5007ed2922996CC5da"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Returns wallet information</response>
    /// <response code="400">When wallet id is invalid</response>
    /// <response code="404">When wallet is not found</response>
    /// <response code="500">When wallet info is not shown due to some error</response>
    [HttpGet("main")]
    [ProducesResponseType(typeof(EthereumWalletResponse), 200)]
    [ProducesResponseType(400), ProducesResponseType(204), ProducesResponseType(500)]
    public async Task<IActionResult> GetEthereumWallet(string? id, string? email, CancellationToken token)
    {
        if (!string.IsNullOrEmpty(id))
        {
            if (!IsParsable(id)) return BadRequest("Wallet id is invalid");
            return Ok(await _mediator.Send(new GetEthereumWalletByIdQuery(id), token));
        }
        if (!string.IsNullOrEmpty(email))
            return Ok(await _mediator.Send(new GetEthereumWalletByEmailQuery(email), token));
        return NotFound();
    }

    /// <summary>
    /// Returns information about user's Ethereum P2P wallet: Ethereum account's address, balance in Ether
    /// </summary>
    /// <param name="id">Wallet's id in ObjectID format</param>
    /// <param name="email">User email</param>
    /// <param name="token"></param>
    /// <returns>P2P wallet's information: address, and balance in Ether</returns>
    /// <remarks>
    /// Sample request:
    ///     GET /eth/6277d227108472b96eee5e56/p2p
    /// 
    /// Sample response:
    /// 
    ///     {
    ///         "balance": 95.58789490,
    ///         "address": "0xD2BE74365557b91070405d5007ed2922996CC5da"
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">Returns wallet information</response>
    /// <response code="400">When wallet id is invalid</response>
    /// <response code="404">When wallet is not found</response>
    /// <response code="500">When wallet info is not shown due to some error</response>
    [HttpGet("p2p")]
    [ProducesResponseType(typeof(EthereumP2PWalletResponse), 200)]
    [ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<IActionResult> GetP2PWallet(string? id, string? email, CancellationToken token)
    {
        if (!string.IsNullOrEmpty(id))
        {
            if (!IsParsable(id)) return BadRequest("Wallet id is invalid");
            return Ok(await _mediator.Send(new GetEthereumP2PWalletByIdQuery(id), token));
        }
        if (!string.IsNullOrEmpty(email))
            return Ok(await _mediator.Send(new GetEthereumP2PWalletByEmailQuery(email), token));
        return NotFound();
    }


    /// <summary>
    /// For lots: set amount of ether to buy in lot. This is the copy of balance in real wallet.
    /// </summary>
    /// <param name="command">Wallet id and amount of ether</param>
    /// <param name="token"></param>
    /// <returns>Status code of setting amount to buy</returns>
    /// <remarks>
    /// Sample request:
    ///     POST /eth/p2p/setToBuy
    /// 
    /// Sample request:
    /// 
    ///     {
    ///         "walletId": "",
    ///         "amount": 95.695
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">When amount to buy is set</response>
    /// <response code="400">When wallet id is invalid or amount is not greater than 0</response>
    /// <response code="404">When wallet is not found</response>
    /// <response code="500">When amount to buy is not set due to server error</response>
    [HttpPut("p2p/set-buy")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<decimal> SetAmountToBuy([FromBody] SetEthAmountToBuyCommand command, CancellationToken token) => 
        await _mediator.Send(command, token);


    /// <summary>
    /// For lots: reduce amount of ether to buy in lot. This is the copy of balance in real wallet.
    /// </summary>
    /// <param name="command">Wallet id and amount of ether</param>
    /// <param name="token"></param>
    /// <returns>Status code of reducing amount to buy</returns>
    /// <remarks>
    /// Sample request:
    ///     POST /eth/p2p/reduceToBuy
    /// 
    /// Sample request:
    /// 
    ///     {
    ///         "walletId": "",
    ///         "amount": 95.695
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">When amount to buy is reduced</response>
    /// <response code="400">When wallet id is invalid or amount is not greater than 0</response>
    /// <response code="404">When wallet is not found</response>
    /// <response code="500">When amount to buy is not reduced due to server error</response>
    [HttpPut("p2p/reduce-buy")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<decimal> ReduceAmountToBuy([FromBody] ReduceAmountToBuyCommand command, CancellationToken token) => 
        await _mediator.Send(command, token);

    /// <summary>
    /// For lots: increase amount of ether to buy in lot. This is the copy of balance in real wallet.
    /// </summary>
    /// <param name="command">Wallet id and amount of ether</param>
    /// <param name="token"></param>
    /// <returns>Status code of increased amount to buy</returns>
    /// <remarks>
    /// Sample request:
    ///     POST /eth/p2p/increaseToBuy
    /// 
    /// Sample request:
    /// 
    ///     {
    ///         "walletId": "",
    ///         "amount": 95.695
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">When amount to buy is increased</response>
    /// <response code="400">When wallet id is invalid or amount is not greater than 0</response>
    /// <response code="404">When wallet is not found</response>
    /// <response code="500">When amount to buy is not increased due to server error</response>
    [HttpPut("p2p/increase-buy")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<decimal> IncreaseAmountToBuy([FromBody] IncreaseAmountToBuyCommand command, CancellationToken token) => 
        await _mediator.Send(command, token);


    /// <summary>
    /// For lots: get amount to buy in lot
    /// </summary>
    /// <param name="id">Wallet id</param>
    /// <param name="token"></param>
    /// <returns>Amount to buy in ether</returns>
    /// <response code="200">Amount to buy</response>
    /// <response code="400">When wallet id is invalid or amount is not greater than 0</response>
    /// <response code="404">When wallet is not found</response>
    /// <response code="500">When amount to buy is not reduced due to server error</response>
    [HttpGet("p2p/{id}/buy-amount")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<decimal?> GetAmountToBuy(string id, CancellationToken token)
    {
        if (!IsParsable(id)) return null;
        return await _mediator.Send(new GetAmountToBuyQuery { WalletId = id }, token);
    }

    /// <summary>
    /// For lots: set amount of ether to sell in lot. This is the copy of balance in real wallet.
    /// </summary>
    /// <param name="command">Wallet id and amount of ether</param>
    /// <param name="token"></param>
    /// <returns>Status code of setting amount to sell</returns>
    /// <remarks>
    /// Sample request:
    ///     POST /eth/p2p/setToSell
    /// 
    /// Sample request:
    /// 
    ///     {
    ///         "walletId": "",
    ///         "amount": 95.695
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">When amount to sell is set</response>
    /// <response code="400">When wallet id is invalid or amount is not greater than 0</response>
    /// <response code="404">When wallet is not found</response>
    /// <response code="500">When amount to sell is not set due to server error</response>
    [HttpPut("p2p/set-sell")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<decimal> SetAmountToSell([FromBody] SetAmountToSellCommand command, CancellationToken token) => 
        await _mediator.Send(command, token);

    /// <summary>
    /// For lots: reduce amount of ether to sell in lot. This is the copy of balance in real wallet.
    /// </summary>
    /// <param name="command">Wallet id and amount of ether</param>
    /// <param name="token"></param>
    /// <returns>Status code of reducing amount to sell</returns>
    /// <remarks>
    /// Sample request:
    ///     POST /eth/p2p/reduceToSell
    /// 
    /// Sample request:
    /// 
    ///     {
    ///         "walletId": "",
    ///         "amount": 95.695
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">When amount to sell is reduced</response>
    /// <response code="400">When wallet id is invalid or amount is not greater than 0</response>
    /// <response code="404">When wallet is not found</response>
    /// <response code="500">When amount to sell is not reduced due to server error</response>
    [HttpPut("p2p/reduce-sell")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<decimal> ReduceAmountToSell([FromBody] ReduceAmountToSellCommand command, CancellationToken token) => 
        await _mediator.Send(command, token);

    /// <summary>
    /// For lots: increase amount of ether to sell in lot. This is the copy of balance in real wallet.
    /// </summary>
    /// <param name="command">Wallet id and amount of ether</param>
    /// <param name="token"></param>
    /// <returns>Status code of increased amount to sell</returns>
    /// <remarks>
    /// Sample request:
    ///     POST /eth/p2p/increaseToSell
    /// 
    /// Sample request:
    /// 
    ///     {
    ///         "walletId": "",
    ///         "amount": 95.695
    ///     }
    /// 
    /// </remarks>
    /// <response code="200">When amount to sell is increased</response>
    /// <response code="400">When wallet id is invalid or amount is not greater than 0</response>
    /// <response code="404">When wallet is not found</response>
    /// <response code="500">When amount to sell is not increased due to server error</response>
    [HttpPut("p2p/increase-sell")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<decimal> IncreaseAmountToSell([FromBody] IncreaseAmountToSellCommand command, CancellationToken token) => 
        await _mediator.Send(command, token);


    /// <summary>
    /// For lots: get amount to sell in lot
    /// </summary>
    /// <param name="id">Wallet id</param>
    /// <param name="token"></param>
    /// <returns>Amount to sell in ether</returns>
    /// <response code="200">Amount to sell</response>
    /// <response code="400">When wallet id is invalid or amount is not greater than 0</response>
    /// <response code="404">When wallet is not found</response>
    /// <response code="500">When amount to sell is not reduced due to server error</response>
    [HttpGet("p2p/{id}/sell-amount")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<decimal?> GetAmountToSell(string id, CancellationToken token)
    {
        if (!IsParsable(id)) return null;
        return await _mediator.Send(new GetAmountToSellQuery { WalletId = id }, token);
    }
}