
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
        var handler = new CadastrarClienteHandler(repositorio);

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
    }

    [Fact]
    public async Task HandleAsync_ComCpfDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var repositorio = new FakeClienteRepository();
        var handler = new CadastrarClienteHandler(repositorio);

        var command = new CadastrarClienteCommand(
            "Maria Silva",
            "52998224725",
            "maria@exemplo.com");

        await handler.HandleAsync(command);

        // Act e Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command));

        Assert.Single(repositorio.Clientes);
    }

    [Fact]
    public async Task HandleAsync_ComNomeInvalido_NaoDeveCadastrar()
    {
        // Arrange
        var repositorio = new FakeClienteRepository();
        var handler = new CadastrarClienteHandler(repositorio);

        var command = new CadastrarClienteCommand(
            "",
            "52998224725",
            "maria@exemplo.com");

        // Act e Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command));

        Assert.Empty(repositorio.Clientes);
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


    [Fact]
    public async Task HandleAsync_ComMesmoCpfEmFormatosDiferentes_DeveImpedirDuplicidade()
    {
        // Arrange
        var repositorio = new FakeClienteRepository();
        var handler = new CadastrarClienteHandler(repositorio);

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
    }

}
