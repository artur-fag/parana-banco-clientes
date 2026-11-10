using Clientes.Infrastructure.Messaging.RabbitMQ;
using Clientes.Application.Interfaces;
using Clientes.Infrastructure.Persistence;
using Clientes.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Clientes.Infrastructure.BackgroundServices;

namespace Clientes.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(
            "ClientesDb");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "A connection string 'ClientesDb' não foi configurada.");
        }

        services.AddDbContext<ClientesDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddScoped<IOutboxEventPublisher, RabbitMqOutboxEventPublisher>();
        services.AddHostedService<OutboxWorker>();
        return services;
    }
}
