
using System.Text.Json;
using Clientes.Application.Events;
using Clientes.Application.Interfaces;
using Clientes.Domain.Entities;

namespace Clientes.Application.UseCases.CadastrarCliente;

public sealed class CadastrarClienteHandler
{
    private readonly IClienteRepository _clienteRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CadastrarClienteHandler(
        IClienteRepository clienteRepository,
        IOutboxRepository outboxRepository,
        IUnitOfWork unitOfWork)
    {
        _clienteRepository = clienteRepository;
        _outboxRepository = outboxRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(
        CadastrarClienteCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var cliente = new Cliente(
            command.Nome,
            command.Cpf,
            command.Email);

        var cpfExistente = await _clienteRepository.ExisteCpfAsync(
            cliente.Cpf,
            cancellationToken);

        if (cpfExistente)
        {
            throw new InvalidOperationException(
                "Já existe um cliente cadastrado com este CPF.");
        }

        var evento = new ClienteCadastradoEvent(
            EventId: Guid.NewGuid(),
            ClienteId: cliente.Id,
            Nome: cliente.Nome,
            Cpf: cliente.Cpf,
            Email: cliente.Email,
            OcorridoEm: DateTime.UtcNow);

        var mensagemOutbox = new OutboxMessage(
            id: evento.EventId,
            tipo: "ClienteCadastrado",
            conteudo: JsonSerializer.Serialize(evento),
            criadoEm: evento.OcorridoEm);

        await _clienteRepository.AdicionarAsync(
            cliente,
            cancellationToken);

        await _outboxRepository.AdicionarAsync(
            mensagemOutbox,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return cliente.Id;
    }
}
