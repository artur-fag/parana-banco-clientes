using System;
using System.Collections.Generic;
using System.Text;

namespace Clientes.Application.UseCases.CadastrarCliente
{
    public sealed record CadastrarClienteCommand(
        string Nome,
        string Cpf,
        string Email
    );

}
