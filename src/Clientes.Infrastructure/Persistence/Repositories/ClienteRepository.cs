
using Clientes.Application.Interfaces;
using Clientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infrastructure.Persistence.Repositories;

public sealed class ClienteRepository : IClienteRepository
{
    private readonly ClientesDbContext _context;

    public ClienteRepository(ClientesDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(
        Cliente cliente,
        CancellationToken cancellationToken = default)
    {
        await _context.Clientes.AddAsync(
            cliente,
            cancellationToken);
    }

    public async Task<Cliente?> ObterPorIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.Id == id,
                cancellationToken);
    }

    public async Task<bool> ExisteCpfAsync(
        string cpf,
        CancellationToken cancellationToken = default)
    {
        return await _context.Clientes
            .AsNoTracking()
            .AnyAsync(
                c => c.Cpf == cpf,
                cancellationToken);
    }
}
