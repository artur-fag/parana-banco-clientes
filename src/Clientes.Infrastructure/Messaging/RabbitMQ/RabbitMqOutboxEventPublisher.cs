
using System.Text;
using Clientes.Application.Interfaces;
using Clientes.Domain.Entities;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Clientes.Infrastructure.Messaging.RabbitMQ;

public sealed class RabbitMqOutboxEventPublisher
    : IOutboxEventPublisher
{
    private readonly RabbitMqOptions _options;

    public RabbitMqOutboxEventPublisher(
        IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
    }

    public async Task PublicarAsync(
        OutboxMessage mensagem,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(mensagem);

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        await using var connection =
            await factory.CreateConnectionAsync(cancellationToken);

        var channelOptions = new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true);

        await using var channel =
            await connection.CreateChannelAsync(
                channelOptions,
                cancellationToken);

        // 1. Cria a exchange.
        await channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        // 2. Cria a fila antes da publicação.
        await channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        // 3. Vincula a fila à exchange.
        await channel.QueueBindAsync(
            queue: _options.QueueName,
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            cancellationToken: cancellationToken);

        // 4. Prepara a mensagem.
        var body = Encoding.UTF8.GetBytes(mensagem.Conteudo);

        var properties = new BasicProperties
        {
            Persistent = true,
            MessageId = mensagem.Id.ToString(),
            Type = mensagem.Tipo,
            ContentType = "application/json"
        };

        // 5. Publica com confirmação do RabbitMQ.
        await channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: _options.RoutingKey,
            mandatory: true,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);
    }
}
