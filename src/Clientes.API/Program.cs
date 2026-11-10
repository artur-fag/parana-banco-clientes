
using Clientes.Application.UseCases.CadastrarCliente;
using Clientes.Application.UseCases.ConsultarClientePorId;
using Clientes.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Infraestrutura
builder.Services.AddInfrastructure(builder.Configuration);

// Casos de uso
builder.Services.AddScoped<CadastrarClienteHandler>();
builder.Services.AddScoped<ConsultarClientePorIdHandler>();

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Documento OpenAPI
    app.MapOpenApi();

    // Interface Swagger UI
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Paraná Banco - Clientes API v1");

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
