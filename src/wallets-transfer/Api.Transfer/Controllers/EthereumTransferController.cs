using Application.Commands.Ethereum.Transfer;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Transfer.Controllers;
/// <summary>
/// Ethereum transfer operations
/// </summary>
[ApiController]
[Route("api/v1/eth/transfer")]
public class EthereumTransferController : ControllerBase
{
    private readonly IMediator _mediator;
    public EthereumTransferController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Transfer Ether from platform wallet to P2P wallet
    /// </summary>
    /// <param name="command">Platform wallet id; Address of P2P wallet (Ethereum account); Amount of ether </param>
    /// <param name="token"></param>
    /// <returns>Transfer to P2P wallet result message</returns>
    /// <response code="200">Ether is successfully transferred to P2P wallet</response>
    /// <response code="400">When wallet id is invalid; when P2P wallet's address is invalid; when amount of ether is less or equal to 0</response>
    /// <response code="500">Error with transferring ether: transaction error</response>
    [HttpPost("to-p2p")]
    [ProducesDefaultResponseType(typeof(TransactionDetails))]
    [ProducesResponseType(200), ProducesResponseType(400), ProducesResponseType(500)]
    public async Task<TransactionDetails> TransferToP2P([FromBody] TransferEthToP2PWalletCommand command, CancellationToken token) 
        => await _mediator.Send(command, token);

    /// <summary>
    /// Refund ether from P2P wallet back to platform's wallet
    /// </summary>
    /// <param name="command">Platform wallet id; Amount of ether </param>
    /// <param name="token"></param>
    /// <returns>Refund result message</returns>
    /// <response code="200">Ether is successfully transferred back to platform wallet</response>
    /// <response code="400">When wallet id is invalid; when amount of ether is less or equal to 0</response>
    /// <response code="500">Error with transferring ether: transaction error</response>
    [HttpPost("to-main")]
    [ProducesDefaultResponseType(typeof(TransactionDetails))]
    [ProducesResponseType(200), ProducesResponseType(400), ProducesResponseType(500)]
    public async Task<TransactionDetails> Refund([FromBody] RefundEthFromP2PWalletCommand command, CancellationToken token) 
        => await _mediator.Send(command, token);

    /// <summary>
    /// Transfer ether from P2P wallet to recipient's wallet
    /// </summary>
    /// <param name="command">P2P wallet id; Address of recipient's wallet (Ethereum account); Amount of ether </param>
    /// <param name="token"></param>
    /// <returns>Transfer to recipient's wallet result message</returns>
    /// <response code="200">Ether are successfully transferred to recipient's wallet</response>
    /// <response code="400">When P2P wallet id is invalid; when recipient's address is invalid; when amount of ether is less or equal to 0</response>
    /// <response code="500">Error with transferring ether: transaction error</response>
    [HttpPost]
    [ProducesDefaultResponseType(typeof(TransactionDetails))]
    [ProducesResponseType(200), ProducesResponseType(400), ProducesResponseType(500)]
    public async Task<TransactionDetails> TransferFromP2P([FromBody] TransferEthFromP2PWalletCommand command, CancellationToken token) 
        => await _mediator.Send(command, token);

}