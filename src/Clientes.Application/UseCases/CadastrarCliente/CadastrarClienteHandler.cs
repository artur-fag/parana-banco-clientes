
using Clientes.Application.Interfaces;
using Clientes.Domain.Entities;

namespace Clientes.Application.UseCases.CadastrarCliente;

public sealed class CadastrarClienteHandler
{
    private readonly IClienteRepository _clienteRepository;

    public CadastrarClienteHandler(
        IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<Guid> HandleAsync(
        CadastrarClienteCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        // Cria e valida a entidade de domínio.
        var cliente = new Cliente(
            command.Nome,
            command.Cpf,
            command.Email);

        // Verifica se o CPF já está cadastrado.
        bool cpfExistente =
            await _clienteRepository.ExisteCpfAsync(
                cliente.Cpf,
                cancellationToken);

        if (cpfExistente)
        {
            throw new InvalidOperationException(
                "Já existe um cliente cadastrado com este CPF.");
        }

        // Solicita a persistência do cliente.
        await _clienteRepository.AdicionarAsync(
            cliente,
            cancellationToken);

        return cliente.Id;
    }
}
