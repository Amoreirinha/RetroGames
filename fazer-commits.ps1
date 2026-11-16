<#
  fazer-commits.ps1 - cria, em ordem, um commit por etapa das Aulas 3 e 4 do RetroGames.

  Uso (na raiz do repositório, com o código do zip já copiado sobre ele):
      .\fazer-commits.ps1            # só commita
      .\fazer-commits.ps1 -Build     # roda dotnet build antes de cada commit
      .\fazer-commits.ps1 -Push      # dá git push no final

  Antes de rodar, instale os pacotes (Aula 4, etapa 1):
      dotnet add RetroGames.WebApi package DotNetEnv
      dotnet add RetroGames.WebApi package Swashbuckle.AspNetCore.SwaggerUI
#>
param(
    [switch]$Build,
    [switch]$Push
)

$ErrorActionPreference = 'Stop'

function Invoke-Git {
    & git.exe @args
    if ($LASTEXITCODE -ne 0) { throw "git.exe $($args -join ' ') falhou." }
}

# --- Verificações iniciais ---------------------------------------------------
if (-not (Test-Path '.git')) { throw 'Rode este script na raiz do repositório (onde fica a pasta .git).' }
if (-not (Test-Path 'RetroGames.slnx')) { throw 'RetroGames.slnx não encontrado: rode na raiz do repositório.' }

$csprojWebApi = Get-Content 'RetroGames.WebApi/RetroGames.WebApi.csproj' -Raw
if ($csprojWebApi -notmatch 'DotNetEnv' -or $csprojWebApi -notmatch 'Swashbuckle') {
    throw "Faltam os pacotes na WebApi. Rode primeiro:`n  dotnet add RetroGames.WebApi package DotNetEnv`n  dotnet add RetroGames.WebApi package Swashbuckle.AspNetCore.SwaggerUI"
}

$programPath = 'RetroGames.WebApi/Program.cs'
$programFinal = [IO.File]::ReadAllText((Resolve-Path $programPath))
$utf8SemBom = New-Object System.Text.UTF8Encoding($false)

# Program.cs das etapas 1 e 2 (ainda sem o GlobalExceptionHandler, que nasce na etapa 3).
$programEtapa1 = @'
using DotNetEnv;
using RetroGames.WebApi.Configuration;

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

var app = builder.Build();

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
'@

function Confirmar {
    param([string]$Mensagem, [string[]]$Caminhos)

    Invoke-Git add -A -- $Caminhos

    & git.exe diff --cached --quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  (nada novo para commitar) $Mensagem" -ForegroundColor Yellow
        return
    }

    if ($Build) {
        Write-Host '  dotnet build...' -ForegroundColor DarkGray
        & dotnet build 'RetroGames.slnx' -v q --nologo
        if ($LASTEXITCODE -ne 0) { throw "O build falhou antes do commit: $Mensagem" }
    }

    Invoke-Git commit -q -m $Mensagem
    Write-Host "  OK  $Mensagem" -ForegroundColor Green
}

try {
    # --- Preparação: segredos e lixo fora do controle de versão ----------------
    Write-Host "`n== Preparação ==" -ForegroundColor Cyan
    Invoke-Git rm --cached -q --ignore-unmatch -- .env
    $lixo = & git.exe ls-files | Where-Object { $_ -match '(^|/)(bin|obj|node_modules)/' }
    foreach ($arquivo in $lixo) { Invoke-Git rm --cached -q -- $arquivo }
    Confirmar 'chore: .gitignore em UTF-8, .env.example corrigido e remoção de .env, bin e obj do controle de versão' `
        @('.gitignore', '.env.example')

    # =========================== AULA 3 - SERVICE ================================
    Write-Host "`n== Aula 3 - Etapa 1: códigos e erros de negócio ==" -ForegroundColor Cyan
    Invoke-Git rm -q --ignore-unmatch -- RetroGames.Services/Class1.cs
    Confirmar 'feat(services): BusinessErrors e BusinessRuleException' @(
        'RetroGames.Services/BusinessErrors.cs',
        'RetroGames.Services/BusinessRuleException.cs')

    Write-Host "`n== Aula 3 - Etapa 2: service de plataformas ==" -ForegroundColor Cyan
    Confirmar 'feat(services): IPlataformaService e PlataformaService' @(
        'RetroGames.Services/IPlataformaService.cs',
        'RetroGames.Services/PlataformaService.cs')

    Write-Host "`n== Aula 3 - Etapa 3: services de gêneros e condições ==" -ForegroundColor Cyan
    Confirmar 'feat(services): services de gêneros e condições de jogo' @(
        'RetroGames.Services/IGeneroService.cs',
        'RetroGames.Services/GeneroService.cs',
        'RetroGames.Services/ICondicaoJogoService.cs',
        'RetroGames.Services/CondicaoJogoService.cs')

    Write-Host "`n== Aula 3 - Etapa 4: service de jogos ==" -ForegroundColor Cyan
    Confirmar 'fix(repository): JogoRepository filtra por ids de jogos (filtro de gênero resolvido no Service)' @(
        'RetroGames.Repository/JogoRepository.cs')
    Confirmar 'feat(services): JogoDetalheServiceModel, IJogoService e JogoService com transação' @(
        'RetroGames.Services/JogoDetalheServiceModel.cs',
        'RetroGames.Services/FiltroJogoServiceModel.cs',
        'RetroGames.Services/IJogoService.cs',
        'RetroGames.Services/JogoService.cs')

    # ============================== AULA 4 - API =================================
    Write-Host "`n== Aula 4 - Etapa 1: API no ar ==" -ForegroundColor Cyan
    [IO.File]::WriteAllText((Resolve-Path $programPath), $programEtapa1, $utf8SemBom)
    Invoke-Git rm -q --ignore-unmatch -- RetroGames.WebApi/WeatherForecast.cs RetroGames.WebApi/Controllers/WeatherForecastController.cs
    Confirmar 'feat(api): Program.cs, pacotes, InjecaoDependenciaConfig e Swagger na porta 5080' @(
        'RetroGames.WebApi/RetroGames.WebApi.csproj',
        'RetroGames.WebApi/Program.cs',
        'RetroGames.WebApi/Configuration',
        'RetroGames.WebApi/Properties/launchSettings.json',
        'RetroGames.WebApi/RetroGames.WebApi.http')

    Write-Host "`n== Aula 4 - Etapa 2: envelope e controller de plataformas ==" -ForegroundColor Cyan
    Invoke-Git rm -q --ignore-unmatch -- RetroGames.WebApi.ApiModels/Class1.cs
    Confirmar 'feat(api): envelope de resposta e PlataformaController' @(
        'RetroGames.WebApi.ApiModels/ResponseCodes.cs',
        'RetroGames.WebApi.ApiModels/RetroGamesResponseApiModel.cs',
        'RetroGames.WebApi.ApiModels/ListarPlataformaResponseApiModel.cs',
        'RetroGames.WebApi/Mappers/PlataformaMapper.cs',
        'RetroGames.WebApi/Controllers/PlataformaController.cs')

    Write-Host "`n== Aula 4 - Etapa 3: tratamento global de erros ==" -ForegroundColor Cyan
    [IO.File]::WriteAllText((Resolve-Path $programPath), $programFinal, $utf8SemBom)
    Confirmar 'feat(api): GlobalExceptionHandler com envelope de erro e status do catálogo' @(
        'RetroGames.WebApi/Middlewares/GlobalExceptionHandler.cs',
        'RetroGames.WebApi/Program.cs')

    Write-Host "`n== Aula 4 - Etapa 4: controllers de gêneros e condições ==" -ForegroundColor Cyan
    Confirmar 'feat(api): GeneroController e CondicaoJogoController' @(
        'RetroGames.WebApi.ApiModels/ListarGeneroResponseApiModel.cs',
        'RetroGames.WebApi.ApiModels/ListarCondicaoJogoResponseApiModel.cs',
        'RetroGames.WebApi/Mappers/GeneroMapper.cs',
        'RetroGames.WebApi/Mappers/CondicaoJogoMapper.cs',
        'RetroGames.WebApi/Controllers/GeneroController.cs',
        'RetroGames.WebApi/Controllers/CondicaoJogoController.cs')

    Write-Host "`n== Aula 4 - Etapa 5: controller de jogos ==" -ForegroundColor Cyan
    Confirmar 'feat(api): JogoController com filtros, paginação e detalhe' @(
        'RetroGames.WebApi.ApiModels/ListarJogoRequestApiModel.cs',
        'RetroGames.WebApi.ApiModels/ListarJogoResponseApiModel.cs',
        'RetroGames.WebApi.ApiModels/ListarPorIdJogoResponseApiModel.cs',
        'RetroGames.WebApi.ApiModels/PaginaResponseApiModel.cs',
        'RetroGames.WebApi/Mappers/JogoMapper.cs',
        'RetroGames.WebApi/Controllers/JogoController.cs')

    # --- Sobras (algo que ficou de fora de todas as etapas) --------------------
    $sobras = & git.exe status --porcelain
    if ($sobras) {
        Write-Host "`nATENÇÃO: ficaram alterações sem commit (confira com git status):" -ForegroundColor Yellow
        $sobras | ForEach-Object { Write-Host "  $_" }
    }

    if ($Push) {
        Write-Host "`n== git push ==" -ForegroundColor Cyan
        Invoke-Git push
    }

    Write-Host "`nPronto. Histórico criado:" -ForegroundColor Cyan
    & git.exe log --oneline -n 15
}
finally {
    # Garante que o Program.cs final volte ao disco mesmo se algo falhar no meio.
    [IO.File]::WriteAllText((Resolve-Path $programPath), $programFinal, $utf8SemBom)
}