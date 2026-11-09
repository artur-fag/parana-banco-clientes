using Clientes.Domain.ValueObjects;

namespace Clientes.UnitTests.ValueObjects;

public class EmailValidatorTests
{
    [Theory]
    [InlineData("cliente@gmail.com")]
    [InlineData("usuario@empresa.com.br")]
    [InlineData("nome.sobrenome@exemplo.com")]
    public void EhValido_ComEmailValido_DeveRetornarTrue(
        string email)
    {
        Assert.True(EmailValidator.EhValido(email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("emailinvalido")]
    [InlineData("cliente@")]
    [InlineData("@gmail.com")]
    [InlineData("cliente@gmail")]
    [InlineData(" cliente@gmail.com ")]
    public void EhValido_ComEmailInvalido_DeveRetornarFalse(
        string? email)
    {
        Assert.False(EmailValidator.EhValido(email));
    }
}
