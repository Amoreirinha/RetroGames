using DotNetEnv;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;

// O .env precisa ser carregado antes do CreateBuilder, para entrar na configuração.
Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string não encontrada. Defina ConnectionStrings__Default no arquivo .env (veja o .env.example).");
}

builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Primeira etapa do pipeline: captura qualquer exceção das etapas seguintes.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opcoes =>
    {
        opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames API v1");
        opcoes.RoutePrefix = "swagger";
    });
}

app.MapControllers();

app.Run();
