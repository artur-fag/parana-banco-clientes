
namespace Clientes.Domain.Entities;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Tipo { get; private set; } = string.Empty;
    public string Conteudo { get; private set; } = string.Empty;
    public DateTime CriadoEm { get; private set; }
    public DateTime? ProcessadoEm { get; private set; }
    public int Tentativas { get; private set; }
    public string? UltimoErro { get; private set; }

    private OutboxMessage()
    {
    }

    public OutboxMessage(
        Guid id,
        string tipo,
        string conteudo,
        DateTime criadoEm)
    {
        if (id == Guid.Empty)
            throw new ArgumentException(
                "O identificador do evento é inválido.",
                nameof(id));

        ArgumentException.ThrowIfNullOrWhiteSpace(tipo);
        ArgumentException.ThrowIfNullOrWhiteSpace(conteudo);

        Id = id;
        Tipo = tipo;
        Conteudo = conteudo;
        CriadoEm = criadoEm;
    }

    public void MarcarComoProcessado()
    {
        ProcessadoEm = DateTime.UtcNow;
        UltimoErro = null;
    }

    public void RegistrarFalha(string erro)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(erro);

        Tentativas++;
        UltimoErro = erro;
    }
}
