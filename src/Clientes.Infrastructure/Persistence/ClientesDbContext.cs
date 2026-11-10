
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

    public DbSet<OutboxMessage> OutboxMessages =>
        Set<OutboxMessage>();

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

        modelBuilder.Entity<OutboxMessage>(entity =>
        {
            entity.ToTable("OutboxMessages");

            entity.HasKey(m => m.Id);

            entity.Property(m => m.Tipo)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(m => m.Conteudo)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            entity.Property(m => m.CriadoEm)
                .IsRequired();

            entity.Property(m => m.ProcessadoEm);

            entity.Property(m => m.Tentativas)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(m => m.UltimoErro)
                .HasColumnType("nvarchar(max)");

            entity.HasIndex(m => new
            {
                m.ProcessadoEm,
                m.CriadoEm
            });
        });
    }
}
