using Clientes.Domain.Entities;

namespace Clientes.UnitTests.Domain.Entities;

public class OutboxMessageTests
{
    [Fact]
    public void NovaMensagem_DeveIniciarPendente()
    {
        var mensagem = CriarMensagem();

        Assert.Null(mensagem.ProcessadoEm);
        Assert.Equal(0, mensagem.Tentativas);
        Assert.Null(mensagem.UltimoErro);
    }

    [Fact]
    public void MarcarComoProcessado_DeveRegistrarData()
    {
        var mensagem = CriarMensagem();

        mensagem.MarcarComoProcessado();

        Assert.NotNull(mensagem.ProcessadoEm);
        Assert.True(mensagem.ProcessadoEm <= DateTime.UtcNow);
        Assert.Null(mensagem.UltimoErro);
    }

    [Fact]
    public void RegistrarFalha_DeveIncrementarTentativas()
    {
        var mensagem = CriarMensagem();

        mensagem.RegistrarFalha("RabbitMQ indisponível");
        mensagem.RegistrarFalha("Falha de conexão");

        Assert.Equal(2, mensagem.Tentativas);
        Assert.Equal("Falha de conexão", mensagem.UltimoErro);
        Assert.Null(mensagem.ProcessadoEm);
    }

    [Fact]
    public void MarcarComoProcessado_DeveLimparUltimoErro()
    {
        var mensagem = CriarMensagem();

        mensagem.RegistrarFalha("Falha temporária");
        mensagem.MarcarComoProcessado();

        Assert.NotNull(mensagem.ProcessadoEm);
        Assert.Null(mensagem.UltimoErro);
        Assert.Equal(1, mensagem.Tentativas);
    }

    private static OutboxMessage CriarMensagem()
    {
        return new OutboxMessage(
            Guid.NewGuid(),
            "ClienteCadastrado",
            """{"clienteId":"123"}""",
            DateTime.UtcNow);
    }
}
