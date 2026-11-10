
using Clientes.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Clientes.Infrastructure.BackgroundServices;

public sealed class OutboxWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxWorker> _logger;

    private static readonly TimeSpan Intervalo =
        TimeSpan.FromSeconds(5);

    public OutboxWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Outbox Worker iniciado.");

        using var timer = new PeriodicTimer(Intervalo);

        do
        {
            try
            {
                await ProcessarMensagensAsync(stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro durante o processamento da Outbox.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task ProcessarMensagensAsync(
        CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var outboxRepository =
            scope.ServiceProvider
                .GetRequiredService<IOutboxRepository>();

        var publisher =
            scope.ServiceProvider
                .GetRequiredService<IOutboxEventPublisher>();

        var unitOfWork =
            scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();

        var mensagens =
            await outboxRepository.ObterPendentesAsync(
                limite: 20,
                cancellationToken: cancellationToken);

        foreach (var mensagem in mensagens)
        {
            try
            {
                await publisher.PublicarAsync(
                    mensagem,
                    cancellationToken);

                mensagem.MarcarComoProcessado();

                await unitOfWork.SaveChangesAsync(
                    cancellationToken);

                _logger.LogInformation(
                    "Evento {EventId} publicado.",
                    mensagem.Id);
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                mensagem.RegistrarFalha(ex.Message);

                await unitOfWork.SaveChangesAsync(
                    cancellationToken);

                _logger.LogError(
                    ex,
                    "Falha ao publicar evento {EventId}. Tentativa {Tentativas}.",
                    mensagem.Id,
                    mensagem.Tentativas);
            }
        }
    }
}
