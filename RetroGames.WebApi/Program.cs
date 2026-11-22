using DotNetEnv;
using FluentValidation;
using RetroGames.Services;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Filters;
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

// O filtro valida o formato de toda entrada antes da action; o ApiBehavior cobre JSON malformado.
builder.Services.AddControllers(opcoes => opcoes.Filters.Add<FluentValidationFilter>())
    .ConfigureApiBehaviorOptions(opcoes =>
        opcoes.InvalidModelStateResponseFactory = ValidacaoConfig.RespostaModelStateInvalido);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// O primeiro administrador nasce na inicialização, com e-mail e senha lidos do .env.
using (var escopo = app.Services.CreateScope())
{
    var autenticacao = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();
    await autenticacao.GarantirAdministradorInicialAsync(
        builder.Configuration["AdminInicial:Email"],
        builder.Configuration["AdminInicial:Senha"]);
}

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
