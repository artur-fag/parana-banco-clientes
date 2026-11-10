
using Clientes.Application.Interfaces;

namespace Clientes.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ClientesDbContext _context;

    public UnitOfWork(ClientesDbContext context)
    {
        _context = context;
    }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }
}
