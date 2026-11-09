
using Clientes.Domain.Entities;

namespace Clientes.Application.Interfaces;

public interface IClienteRepository
{
    Task AdicionarAsync(
        Cliente cliente,
        CancellationToken cancellationToken = default);

    Task<Cliente?> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExisteCpfAsync(
        string cpf,
        CancellationToken cancellationToken = default);
}
