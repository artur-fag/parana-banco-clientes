using Clientes.Domain.Entities;

namespace Clientes.Application.Interfaces;

public interface IOutboxRepository
{
    Task AdicionarAsync(
        OutboxMessage mensagem,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OutboxMessage>> ObterPendentesAsync(
        int limite,
        CancellationToken cancellationToken = default);
}
