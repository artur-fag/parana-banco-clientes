using Clientes.Domain.Entities;

namespace Clientes.Application.Interfaces;

public interface IOutboxEventPublisher
{
    Task PublicarAsync(
        OutboxMessage mensagem,
        CancellationToken cancellationToken = default);
}
