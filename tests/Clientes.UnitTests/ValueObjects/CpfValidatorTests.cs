
using Clientes.Domain.ValueObjects;

namespace Clientes.UnitTests.ValueObjects;

public class CpfValidatorTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    public void EhValido_ComCpfValido_DeveRetornarTrue(string cpf)
    {
        // Act
        bool resultado = CpfValidator.EhValido(cpf);

        // Assert
        Assert.True(resultado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("12345678900")]
    [InlineData("11111111111")]
    [InlineData("123456789")]
    public void EhValido_ComCpfInvalido_DeveRetornarFalse(string? cpf)
    {
        // Act
        bool resultado = CpfValidator.EhValido(cpf);

        // Assert
        Assert.False(resultado);
    }


    [Theory]
    [InlineData("abc52998224725")]
    [InlineData("52998224725xyz")]
    [InlineData("529-982-247-25")]
    [InlineData("529.982.247/25")]
    [InlineData("5299822472A")]
    public void EhValido_ComCaracteresInvalidos_DeveRetornarFalse(
        string cpf)
    {
        bool resultado = CpfValidator.EhValido(cpf);

        Assert.False(resultado);
    }

}
