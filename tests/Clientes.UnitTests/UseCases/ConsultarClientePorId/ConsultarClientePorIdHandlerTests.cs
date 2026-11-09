using Clientes.Application.Interfaces;
using Clientes.Application.UseCases.ConsultarClientePorId;
using Clientes.Domain.Entities;

namespace Clientes.UnitTests.UseCases.ConsultarClientePorId;

public class ConsultarClientePorIdHandlerTests
{
    [Fact]
    public async Task HandleAsync_ClienteExistente_DeveRetornarCliente()
    {
        
        var cliente = new Cliente(
            "Maria Silva",
            "52998224725",
            "maria@exemplo.com");

        var repositorio = new FakeClienteRepository(cliente);
        var handler = new ConsultarClientePorIdHandler(repositorio);

        
        var resultado = await handler.HandleAsync(cliente.Id);

       
        Assert.NotNull(resultado);
        Assert.Equal(cliente.Id, resultado.Id);
        Assert.Equal(cliente.Nome, resultado.Nome);
        Assert.Equal(cliente.Cpf, resultado.Cpf);
        Assert.Equal(cliente.Email, resultado.Email);
        Assert.Equal(cliente.CriadoEm, resultado.CriadoEm);
    }

    [Fact]
    public async Task HandleAsync_ClienteInexistente_DeveRetornarNull()
    {
        
        var repositorio = new FakeClienteRepository();
        var handler = new ConsultarClientePorIdHandler(repositorio);

       
        var resultado = await handler.HandleAsync(Guid.NewGuid());

     
        Assert.Null(resultado);
    }

    [Fact]
    public async Task HandleAsync_IdVazio_DeveLancarExcecao()
    {
            var repositorio = new FakeClienteRepository();
        var handler = new ConsultarClientePorIdHandler(repositorio);

        
        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(Guid.Empty));
    }

    private sealed class FakeClienteRepository : IClienteRepository
    {
        private readonly Cliente? _cliente;

        public FakeClienteRepository(Cliente? cliente = null)
        {
            _cliente = cliente;
        }

        public Task<Cliente?> ObterPorIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var resultado = _cliente?.Id == id ? _cliente : null;

            return Task.FromResult(resultado);
        }

        public Task AdicionarAsync(
            Cliente cliente,
            CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<bool> ExisteCpfAsync(
            string cpf,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }
    }
}
