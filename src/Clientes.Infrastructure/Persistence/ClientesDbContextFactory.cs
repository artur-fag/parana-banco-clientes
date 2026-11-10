
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Clientes.Infrastructure.Persistence;

public sealed class ClientesDbContextFactory
    : IDesignTimeDbContextFactory<ClientesDbContext>
{
    public ClientesDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets(
                "5c6b3ef9-4a54-491d-bebd-a3447da1a33e")
            .Build();

        var connectionString = configuration.GetConnectionString(
            "ClientesDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'ClientesDb' não configurada.");
        }

        var optionsBuilder =
            new DbContextOptionsBuilder<ClientesDbContext>();

        optionsBuilder.UseSqlServer(connectionString);

        return new ClientesDbContext(optionsBuilder.Options);
    }
}
