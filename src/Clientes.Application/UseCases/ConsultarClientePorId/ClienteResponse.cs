namespace Clientes.Application.UseCases.ConsultarClientePorId;

public sealed record ClienteResponse(
    Guid Id,
    string Nome,
    string Cpf,
    string Email,
    DateTime CriadoEm
);
