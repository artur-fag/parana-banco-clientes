using Clientes.Domain.ValueObjects;

namespace Clientes.Domain.Entities;

public sealed class Cliente
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public string Cpf { get; private set; }
    public string Email { get; private set; }
    public DateTime CriadoEm { get; private set; }

    private Cliente()
    {
        Nome = string.Empty;
        Cpf = string.Empty;
        Email = string.Empty;
    }

public Cliente(string nome, string cpf, string email)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException(
                "O Campo Nome é obrigatório.",
                nameof(nome));

        if (!CpfValidator.EhValido(cpf))
            throw new ArgumentException(
                "CPF inválido.",
                nameof(cpf));

        if (!EmailValidator.EhValido(email))
            throw new ArgumentException(
                "E-mail inválido.",
                nameof(email));

        Id = Guid.NewGuid();
        Nome = nome.Trim();
        Cpf = cpf.Replace(".", "").Replace("-", "");
        Email = email;
        CriadoEm = DateTime.UtcNow;
    }

}
