
using System.Text.Json;
using Clientes.Application.Events;
using Clientes.Application.Interfaces;
using Clientes.Application.UseCases.CadastrarCliente;
using Clientes.Domain.Entities;

namespace Clientes.UnitTests.UseCases.CadastrarCliente;

public class CadastrarClienteHandlerTests
{
    [Fact]
    public async Task HandleAsync_ComDadosValidos_DeveCadastrarCliente()
    {
        // Arrange
        var repositorio = new FakeClienteRepository();
        var outbox = new FakeOutboxRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CadastrarClienteHandler(
            repositorio,
            outbox,
            unitOfWork);

        var command = new CadastrarClienteCommand(
            "Maria Silva",
            "52998224725",
            "maria@exemplo.com");

        // Act
        Guid clienteId = await handler.HandleAsync(command);

        // Assert
        Assert.NotEqual(Guid.Empty, clienteId);
        Assert.Single(repositorio.Clientes);
        Assert.Equal(clienteId, repositorio.Clientes[0].Id);

        Assert.Single(outbox.Mensagens);
        Assert.Equal(1, unitOfWork.QuantidadeSalvamentos);

        var mensagem = outbox.Mensagens[0];

        Assert.Equal("ClienteCadastrado", mensagem.Tipo);
        Assert.Null(mensagem.ProcessadoEm);
        Assert.Equal(0, mensagem.Tentativas);

        var evento = JsonSerializer.Deserialize<ClienteCadastradoEvent>(
            mensagem.Conteudo);

        Assert.NotNull(evento);
        Assert.Equal(clienteId, evento.ClienteId);
        Assert.Equal(mensagem.Id, evento.EventId);
        Assert.Equal("Maria Silva", evento.Nome);
        Assert.Equal("52998224725", evento.Cpf);
        Assert.Equal("maria@exemplo.com", evento.Email);
    }

    [Fact]
    public async Task HandleAsync_ComCpfDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var repositorio = new FakeClienteRepository();
        var outbox = new FakeOutboxRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CadastrarClienteHandler(
            repositorio,
            outbox,
            unitOfWork);

        var command = new CadastrarClienteCommand(
            "Maria Silva",
            "52998224725",
            "maria@exemplo.com");

        await handler.HandleAsync(command);

        // Act e Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command));

        Assert.Single(repositorio.Clientes);
        Assert.Single(outbox.Mensagens);
        Assert.Equal(1, unitOfWork.QuantidadeSalvamentos);
    }

    [Fact]
    public async Task HandleAsync_ComNomeInvalido_NaoDeveCadastrar()
    {
        // Arrange
        var repositorio = new FakeClienteRepository();
        var outbox = new FakeOutboxRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CadastrarClienteHandler(
            repositorio,
            outbox,
            unitOfWork);

        var command = new CadastrarClienteCommand(
            "",
            "52998224725",
            "maria@exemplo.com");

        // Act e Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command));

        Assert.Empty(repositorio.Clientes);
        Assert.Empty(outbox.Mensagens);
        Assert.Equal(0, unitOfWork.QuantidadeSalvamentos);
    }

    [Fact]
    public async Task HandleAsync_ComMesmoCpfEmFormatosDiferentes_DeveImpedirDuplicidade()
    {
        // Arrange
        var repositorio = new FakeClienteRepository();
        var outbox = new FakeOutboxRepository();
        var unitOfWork = new FakeUnitOfWork();

        var handler = new CadastrarClienteHandler(
            repositorio,
            outbox,
            unitOfWork);

        var primeiroCadastro = new CadastrarClienteCommand(
            "Maria Silva",
            "52998224725",
            "maria@exemplo.com");

        var segundoCadastro = new CadastrarClienteCommand(
            "Maria Silva",
            "529.982.247-25",
            "maria2@exemplo.com");

        // Act
        await handler.HandleAsync(primeiroCadastro);

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(segundoCadastro));

        Assert.Single(repositorio.Clientes);
        Assert.Equal(
            "52998224725",
            repositorio.Clientes[0].Cpf);

        Assert.Single(outbox.Mensagens);
        Assert.Equal(1, unitOfWork.QuantidadeSalvamentos);
    }

    private sealed class FakeClienteRepository : IClienteRepository
    {
        public List<Cliente> Clientes { get; } = new();

        public Task AdicionarAsync(
            Cliente cliente,
            CancellationToken cancellationToken = default)
        {
            Clientes.Add(cliente);
            return Task.CompletedTask;
        }

        public Task<Cliente?> ObterPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                Clientes.FirstOrDefault(c => c.Id == id));
        }

        public Task<bool> ExisteCpfAsync(
            string cpf,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                Clientes.Any(c => c.Cpf == cpf));
        }
    }


    private sealed class FakeOutboxRepository : IOutboxRepository
    {
        public List<OutboxMessage> Mensagens { get; } = new();

        public Task AdicionarAsync(
            OutboxMessage mensagem,
            CancellationToken cancellationToken = default)
        {
            Mensagens.Add(mensagem);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<OutboxMessage>> ObterPendentesAsync(
            int limite,
            CancellationToken cancellationToken = default)
        {
            if (limite <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(limite),
                    "O limite deve ser maior que zero.");
            }

            IReadOnlyList<OutboxMessage> pendentes = Mensagens
                .Where(m => m.ProcessadoEm == null)
                .OrderBy(m => m.CriadoEm)
                .Take(limite)
                .ToList();

            return Task.FromResult(pendentes);
        }
    }


    private sealed class FakeUnitOfWork : IUnitOfWork
    {
        public int QuantidadeSalvamentos { get; private set; }

        public Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            QuantidadeSalvamentos++;
            return Task.FromResult(1);
        }
    }
}
