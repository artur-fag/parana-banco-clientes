using Clientes.Application.Events;

namespace Clientes.Application.Interfaces;

public interface IClienteEventPublisher
{
    Task PublicarAsync(
        ClienteCadastradoEvent evento,
        CancellationToken cancellationToken = default);
}
