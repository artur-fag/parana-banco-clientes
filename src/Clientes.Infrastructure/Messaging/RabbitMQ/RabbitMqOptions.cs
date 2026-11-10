
namespace Clientes.Infrastructure.Messaging.RabbitMQ;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    public string HostName { get; set; } = "localhost";

    public int Port { get; set; } = 5672;

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string ExchangeName { get; set; } =
        "paranabanco.clientes.events";

    public string RoutingKey { get; set; } =
        "cliente.cadastrado";

    public string QueueName { get; set; } =
        "paranabanco.propostas.cliente-cadastrado";
}
