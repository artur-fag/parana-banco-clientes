
using Clientes.Application.Interfaces;

namespace Clientes.Application.UseCases.ConsultarClientePorId;

public sealed class ConsultarClientePorIdHandler
{
    private readonly IClienteRepository _clienteRepository;

    public ConsultarClientePorIdHandler(
        IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteResponse?> HandleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException(
                "O identificador do cliente é inválido.",
                nameof(id));
        }

        var cliente = await _clienteRepository.ObterPorIdAsync(
            id,
            cancellationToken);

        if (cliente is null)
            return null;

        return new ClienteResponse(
            cliente.Id,
            cliente.Nome,
            cliente.Cpf,
            cliente.Email,
            cliente.CriadoEm);
    }
}
