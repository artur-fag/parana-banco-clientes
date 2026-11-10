
using Clientes.Application.Interfaces;
using Clientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infrastructure.Persistence.Repositories;

public sealed class OutboxRepository : IOutboxRepository
{
    private readonly ClientesDbContext _context;

    public OutboxRepository(ClientesDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(
        OutboxMessage mensagem,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mensagem);

        await _context.OutboxMessages.AddAsync(
            mensagem,
            cancellationToken);
    }

    public async Task<IReadOnlyList<OutboxMessage>> ObterPendentesAsync(
        int limite,
        CancellationToken cancellationToken = default)
    {
        if (limite <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(limite),
                "O limite deve ser maior que zero.");
        }

        return await _context.OutboxMessages
            .Where(m => m.ProcessadoEm == null)
            .OrderBy(m => m.CriadoEm)
            .Take(limite)
            .ToListAsync(cancellationToken);
    }
}
