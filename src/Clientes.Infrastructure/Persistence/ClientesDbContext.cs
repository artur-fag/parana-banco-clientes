using Clientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infrastructure.Persistence;

public sealed class ClientesDbContext : DbContext
{
    public ClientesDbContext(
        DbContextOptions<ClientesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("Clientes");

            entity.HasKey(c => c.Id);

            entity.Property(c => c.Nome)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(c => c.Cpf)
                .IsRequired()
                .HasMaxLength(14);

            entity.HasIndex(c => c.Cpf)
                .IsUnique();

            entity.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(254);

            entity.Property(c => c.CriadoEm)
                .IsRequired();
        });
    }
}
