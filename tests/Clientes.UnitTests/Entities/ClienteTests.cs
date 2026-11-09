
using Clientes.Domain.Entities;

namespace Clientes.UnitTests.Entities;

public class ClienteTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CriarCliente_ComNomeInvalido_DeveLancarExcecao(
        string? nome)
    {
        
        string cpf = "52998224725";
        string email = "cliente@exemplo.com";

    
        var exception = Assert.Throws<ArgumentException>(() =>
            new Cliente(nome!, cpf, email));

      
        Assert.Equal("nome", exception.ParamName);
    }

    [Fact]
    public void CriarCliente_ComNomeValido_DeveCriarCliente()
    {
        
        string nome = "Maria Silva";
        string cpf = "52998224725";
        string email = "maria@exemplo.com";

       
        var cliente = new Cliente(nome, cpf, email);

       
        Assert.NotEqual(Guid.Empty, cliente.Id);
        Assert.Equal(nome, cliente.Nome);
        Assert.Equal(cpf, cliente.Cpf);
        Assert.Equal(email, cliente.Email);
    }


    [Theory]
    [InlineData("12345678900")]
    [InlineData("11111111111")]
    [InlineData("")]
    [InlineData(null)]
    public void CriarCliente_ComCpfInvalido_DeveLancarExcecao(
        string? cpf)
    {
     
        string nome = "Maria Silva";
        string email = "maria@exemplo.com";

     
        var exception = Assert.Throws<ArgumentException>(() =>
            new Cliente(nome, cpf!, email));

        Assert.Equal("cpf", exception.ParamName);
    }

    [Theory]
    [InlineData("emailinvalido")]
    [InlineData("cliente@")]
    [InlineData("")]
    [InlineData(null)]
    public void CriarCliente_ComEmailInvalido_DeveLancarExcecao(
        string? email)
    {
        
        string nome = "Maria Silva";
        string cpf = "52998224725";

        
        var exception = Assert.Throws<ArgumentException>(() =>
            new Cliente(nome, cpf, email!));

    
        Assert.Equal("email", exception.ParamName);
    }


    [Fact]
    public void CriarCliente_ComCpfFormatado_DeveArmazenarSomenteNumeros()
    {
      
        string nome = "Maria Silva";
        string cpf = "529.982.247-25";
        string email = "maria@exemplo.com";

        var cliente = new Cliente(nome, cpf, email);

        Assert.Equal("52998224725", cliente.Cpf);
    }


}
