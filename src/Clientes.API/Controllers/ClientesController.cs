
using Clientes.Application.UseCases.CadastrarCliente;
using Clientes.Application.UseCases.ConsultarClientePorId;
using Microsoft.AspNetCore.Mvc;

namespace Clientes.API.Controllers;

[ApiController]
[Route("api/v1/clientes")]
public sealed class ClientesController : ControllerBase
{
    private readonly CadastrarClienteHandler _cadastrarClienteHandler;
    private readonly ConsultarClientePorIdHandler _consultarClientePorIdHandler;

    public ClientesController(
        CadastrarClienteHandler cadastrarClienteHandler,
        ConsultarClientePorIdHandler consultarClientePorIdHandler)
    {
        _cadastrarClienteHandler = cadastrarClienteHandler;
        _consultarClientePorIdHandler = consultarClientePorIdHandler;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CadastrarAsync(
        [FromBody] CadastrarClienteCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var clienteId = await _cadastrarClienteHandler.HandleAsync(
                command,
                cancellationToken);

            return CreatedAtRoute(
    "ObterClientePorId",
    new { id = clienteId },
    new { id = clienteId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    //[HttpGet("{id:guid}")]
    [HttpGet("{id:guid}", Name = "ObterClientePorId")]
    [ProducesResponseType(typeof(ClienteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteResponse>> ConsultarPorIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        try
        {
            var cliente = await _consultarClientePorIdHandler.HandleAsync(
                id,
                cancellationToken);

            if (cliente is null)
                return NotFound(new
                {
                    mensagem = "Cliente não encontrado."
                });

            return Ok(cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                mensagem = ex.Message
            });
        }
    }
}
