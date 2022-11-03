using Application.Commands.Ethereum.Wallets;
using Application.Responses.Ethereum.Wallets;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Wallets.Api.Controllers;

/// <summary>
/// API for managing Ethereum wallets
/// </summary>
[ApiController]
[Route("api/v1/wallets")]
public class BaseWalletsController : ControllerBase
{
    private readonly IMediator _mediator;
    public BaseWalletsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Create wallet (Ethereum account) with zero balance
    /// </summary>
    /// <param name="command">User's login and password </param>
    /// <param name="token"></param>
    /// <returns>Created wallet with wallet's ID, assigned address and private key</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /create
    ///     {
    ///         "email": "testemail@gmail.com",
    ///         "password": "test_password123"
    ///     }
    /// Sample response:
    /// 
    ///     {
    ///         "address": "0xD2BE74365557b91070405d5007ed2922996CC5da",
    ///         "privateKey": "e13461ef741ab5f0367707d7f0e539b11c2957888888727a8da26e9e60a9a19f",
    ///         "id": "6277d227108472b96eee5e56"
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Returns newly created wallet</response>
    /// <response code="400">When user's email or password are invalid</response>
    /// <response code="500">When user's wallet couldn't be created</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(CreatedEthereumWalletResponse), 201)]
    [ProducesResponseType(400), ProducesResponseType(500)]
    public async Task<IActionResult> CreateWallet([FromBody] CreateEthereumWalletCommand command, CancellationToken token)
    {
        var createdWallet = await _mediator.Send(command, token);
        return CreatedAtRoute(new { id = createdWallet.Id, privateKey = createdWallet.PrivateKey, address = createdWallet.Address }, createdWallet);
    }
    /// <summary>
    /// Load existing wallet (Ethereum account) by private key
    /// </summary>
    /// <param name="command">User's login, password and provided private key of an account</param>
    /// <param name="token"></param>
    /// <returns>Created (loaded) wallet with wallet's ID, assigned address and private key</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /load
    ///     {
    ///         "email": "testemail@gmail.com",
    ///         "password": "test_password123",
    ///         "privateKey": "e13461ef741ab5f0367707d7f0e539b11c2957888888727a8da26e9e60a9a19f"
    ///     }
    /// Sample response:
    /// 
    ///     {
    ///         "address": "0xD2BE74365557b91070405d5007ed2922996CC5da",
    ///         "privateKey": "e13461ef741ab5f0367707d7f0e539b11c2957888888727a8da26e9e60a9a19f",
    ///         "id": "6277d227108472b96eee5e56"
    ///     }
    /// 
    /// </remarks>
    /// <response code="201">Returns loaded wallet</response>
    /// <response code="400">When user's email or password are invalid; When provided private key is invalid</response>
    /// <response code="500">When user's wallet cannot be loaded due to some error</response>
    [HttpPost("load")]
    [ProducesResponseType(typeof(CreatedEthereumWalletResponse), 201)]
    [ProducesResponseType(400), ProducesResponseType(500)]
    public async Task<IActionResult> LoadWallet([FromBody] LoadEthereumWalletCommand command, CancellationToken token)
    {
        var loadedWallet = await _mediator.Send(command, token);
        return CreatedAtRoute(new { id = loadedWallet.Id, privateKey = loadedWallet.PrivateKey, address = loadedWallet.Address }, loadedWallet);
    }

    /// <summary>
    /// Lock Ethereum wallets of user
    /// </summary>
    /// <param name="email">User wallet's id</param>
    /// <param name="token"></param>
    /// <returns>Status code whether the wallets are frozen or not</returns>
    /// <remarks>
    /// WARNING: only admin has permission to lock accounts
    /// 
    /// </remarks>
    /// <response code="200">Wallets are successfully frozen</response>
    /// <response code="400">When wallet id is invalid</response>
    /// <response code="500">When user's wallet cannot be loaded due to some error</response>
    [HttpPatch("lock")]
    [ProducesResponseType(200), ProducesResponseType(400), ProducesResponseType(401), ProducesResponseType(500)]
    public async Task<bool> LockWallet(string email, CancellationToken token)
    => await _mediator.Send(new LockEthereumWalletCommand(email), token);
}