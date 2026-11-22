<#
  fazer-commits-aulas-5-a-7.ps1
  Aplica, em ordem, as etapas das Aulas 5, 6 e 7 do RetroGames e cria um commit por etapa.

  Como funciona: a pasta _etapas (ao lado deste script) tem um pacote de arquivos por etapa.
  Para cada etapa, o script copia os arquivos para o projeto, instala os pacotes que ela pede
  e faz o commit. Assim, cada commit tem exatamente o código daquela etapa.

  Uso (na raiz do repositório; o script e a pasta _etapas ficam nessa raiz):
      .\fazer-commits-aulas-5-a-7.ps1            # aplica e commita
      .\fazer-commits-aulas-5-a-7.ps1 -Build     # compila (dotnet build / npm run build) antes de cada commit
      .\fazer-commits-aulas-5-a-7.ps1 -Push      # dá git push no final

  Pode ser executado de novo: etapas já aplicadas não geram commit vazio.
  Pré-requisitos: Aulas 3 e 4 já commitadas, .NET SDK 10, Node.js 22+ e acesso à internet (NuGet e npm).
#>
param(
    [switch]$Build,
    [switch]$Push
)

$ErrorActionPreference = 'Stop'

$raiz = $PSScriptRoot
if (-not $raiz) { $raiz = (Get-Location).Path }
Set-Location $raiz
$pastaEtapas = Join-Path $raiz '_etapas'

# --- Funções auxiliares -------------------------------------------------------
function Invoke-Git {
    & git.exe @args
    if ($LASTEXITCODE -ne 0) { throw "git.exe $($args -join ' ') falhou." }
}

function Executar {
    param([string]$Exe, [string[]]$Argumentos, [string]$Pasta = '.')
    Push-Location $Pasta
    try {
        & $Exe @Argumentos
        if ($LASTEXITCODE -ne 0) { throw "$Exe $($Argumentos -join ' ') falhou." }
    }
    finally { Pop-Location }
}

# Adiciona um pacote NuGet ao projeto, a menos que ele já esteja no .csproj.
function Garantir-PacoteNuget {
    param([string]$Projeto, [string]$Pacote, [string]$Versao = '')
    $csproj = Get-Content "$Projeto/$Projeto.csproj" -Raw
    if ($csproj -match [regex]::Escape($Pacote)) { return }
    $argumentos = @('add', $Projeto, 'package', $Pacote)
    if ($Versao) { $argumentos += @('--version', $Versao) }
    Executar 'dotnet' $argumentos
}

function Confirmar-Etapa {
    param($Etapa)

    $origem = Join-Path $pastaEtapas $Etapa.Pasta
    if (-not (Test-Path $origem)) { throw "Pasta da etapa não encontrada: $origem" }

    # 1) Pacotes que a etapa pede
    if ($Etapa.Antes) { & $Etapa.Antes }

    # 2) Copia os arquivos da etapa para o projeto
    Copy-Item -Path (Join-Path $origem '*') -Destination $raiz -Recurse -Force

    # 3) Remove os arquivos que a etapa apaga (exemplo do template do Vite)
    foreach ($arquivo in $Etapa.Remover) {
        if (Test-Path $arquivo) { Remove-Item $arquivo -Force }
        Invoke-Git rm --cached -q --ignore-unmatch -- $arquivo
    }

    # 4) Prepara o commit: só os arquivos da etapa (e os extras dela), nunca o .env
    $arquivosDaEtapa = @(Get-ChildItem $origem -Recurse -File |
        ForEach-Object { $_.FullName.Substring($origem.Length + 1).Replace('\', '/') })
    Invoke-Git add -A -- ($arquivosDaEtapa + @($Etapa.Extras))

    & git.exe diff --cached --quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Host "  (nada novo para commitar) $($Etapa.Mensagem)" -ForegroundColor Yellow
        return
    }

    # 5) Opcional: compila antes de commitar (a pasta de trabalho == o conteúdo do commit)
    if ($Build) {
        Write-Host '  compilando...' -ForegroundColor DarkGray
        if ($Etapa.Tipo -eq 'web') {
            Executar 'npm' @('run', 'build') 'retrogames.web'
        }
        else {
            & dotnet build 'RetroGames.slnx' -v q --nologo
            if ($LASTEXITCODE -ne 0) { throw "O build falhou: $($Etapa.Mensagem)" }
        }
    }

    Invoke-Git commit -q -m $Etapa.Mensagem
    Write-Host "  OK  $($Etapa.Mensagem)" -ForegroundColor Green
}

# --- Verificações iniciais ----------------------------------------------------
if (-not (Test-Path '.git')) { throw 'Rode este script na raiz do repositório (onde fica a pasta .git).' }
if (-not (Test-Path $pastaEtapas)) { throw 'A pasta _etapas não está ao lado do script.' }
if (-not (Test-Path 'RetroGames.WebApi/Program.cs') -or -not (Test-Path 'RetroGames.Services/JogoService.cs')) {
    throw 'Não encontrei o código das Aulas 3 e 4. Conclua e commite essas aulas antes.'
}
if (-not (Get-Command npm -ErrorAction SilentlyContinue)) { throw 'npm não encontrado: instale o Node.js 22 LTS.' }

# A versão do pacote de identidade acompanha a do ASP.NET Core já usado no projeto.
$csprojWeb = Get-Content 'RetroGames.WebApi/RetroGames.WebApi.csproj' -Raw
$versaoAspNet = [regex]::Match($csprojWeb, 'Microsoft\.AspNetCore\.OpenApi"\s+Version="([^"]+)"').Groups[1].Value

# --- Definição das etapas, na ordem dos roteiros ------------------------------
$etapas = @(
    @{
        Pasta = '01-aula5-e1-validacao'; Tipo = 'api'
        Mensagem = 'feat(api): validação de entrada com FluentValidation e resposta 400 com código 3'
        Antes = { Garantir-PacoteNuget 'RetroGames.WebApi' 'FluentValidation.DependencyInjectionExtensions' }
        Extras = @('RetroGames.WebApi/RetroGames.WebApi.csproj'); Remover = @()
    },
    @{
        Pasta = '02-aula5-e2-plataformas'; Tipo = 'api'
        Mensagem = 'feat(api): POST, PUT e DELETE de plataformas'
        Antes = $null; Extras = @(); Remover = @()
    },
    @{
        Pasta = '03-aula5-e3-jogos'; Tipo = 'api'
        Mensagem = 'feat(api): POST, PUT e DELETE de jogos, com gêneros e exclusão lógica (RN03)'
        Antes = $null; Extras = @(); Remover = @()
    },
    @{
        Pasta = '04-aula5-e4-respostas'; Tipo = 'api'
        Mensagem = 'test(api): roteiro .http com as respostas do contrato de gravação'
        Antes = $null; Extras = @(); Remover = @()
    },
    @{
        Pasta = '05-aula6-e1-usuarios'; Tipo = 'api'
        Mensagem = 'feat(auth): usuários com senha em hash e administrador inicial'
        Antes = {
            Garantir-PacoteNuget 'RetroGames.Services' 'Microsoft.Extensions.Identity.Core' $versaoAspNet
            if (Test-Path '.env') {
                $conteudoEnv = Get-Content '.env' -Raw
                if ($conteudoEnv -notmatch 'AdminInicial__Email' -or $conteudoEnv -notmatch 'AdminInicial__Senha') {
                    Write-Host '  ATENÇÃO: acrescente AdminInicial__Email e AdminInicial__Senha ao seu .env (veja o .env.example) antes de subir a API.' -ForegroundColor Yellow
                }
            }
        }
        Extras = @('RetroGames.Services/RetroGames.Services.csproj'); Remover = @()
    },
    @{
        Pasta = '06-aula6-e2-sessao-login'; Tipo = 'api'
        Mensagem = 'feat(auth): cookie de sessão e AutenticacaoController (login, logout e eu)'
        Antes = $null; Extras = @(); Remover = @()
    },
    @{
        Pasta = '07-aula6-e3-protecao'; Tipo = 'api'
        Mensagem = 'feat(auth): política padrão, SomenteAdministrador e RN04 (inativos só para o administrador)'
        Antes = $null; Extras = @(); Remover = @()
    },
    @{
        Pasta = '08-aula6-e4-respostas'; Tipo = 'api'
        Mensagem = 'test(auth): roteiro .http com as respostas de acesso'
        Antes = $null; Extras = @(); Remover = @()
    },
    @{
        Pasta = '09-aula7-e1-projeto-react'; Tipo = 'web'
        Mensagem = 'feat(web): projeto React com Vite, Bootstrap, Axios, React Router e proxy para a API'
        Antes = {
            $pacote = Get-Content 'retrogames.web/package.json' -Raw
            if ($pacote -notmatch '"axios"' -or $pacote -notmatch '"bootstrap"' -or $pacote -notmatch '"react-router-dom"') {
                Executar 'npm' @('install', 'axios', 'bootstrap', 'react-router-dom') 'retrogames.web'
            }
        }
        Extras = @('retrogames.web')
        Remover = @(
            'retrogames.web/src/App.css',
            'retrogames.web/src/index.css',
            'retrogames.web/src/assets/hero.png',
            'retrogames.web/src/assets/react.svg',
            'retrogames.web/src/assets/vite.svg',
            'retrogames.web/public/icons.svg'
        )
    },
    @{
        Pasta = '10-aula7-e2-acesso-api'; Tipo = 'web'
        Mensagem = 'feat(web): camada de acesso à API (api.js, servicos.js, erros.js e formato.js)'
        Antes = $null; Extras = @(); Remover = @()
    },
    @{
        Pasta = '11-aula7-e3-layout-rotas'; Tipo = 'web'
        Mensagem = 'feat(web): Layout, rotas do React Router e componentes de estado'
        Antes = $null; Extras = @(); Remover = @()
    },
    @{
        Pasta = '12-aula7-e4-catalogo'; Tipo = 'web'
        Mensagem = 'feat(web): catálogo com busca, filtros na URL e paginação'
        Antes = $null; Extras = @(); Remover = @()
    },
    @{
        Pasta = '13-aula7-e5-detalhe'; Tipo = 'web'
        Mensagem = 'feat(web): página de detalhe do jogo'
        Antes = $null; Extras = @(); Remover = @()
    }
)

# --- Execução -----------------------------------------------------------------
$titulos = @{
    '01' = 'Aula 5 · Etapa 1: validação de entrada'
    '02' = 'Aula 5 · Etapa 2: gravação de plataformas'
    '03' = 'Aula 5 · Etapa 3: gravação de jogos'
    '04' = 'Aula 5 · Etapa 4: todas as respostas da API'
    '05' = 'Aula 6 · Etapa 1: usuários e senhas'
    '06' = 'Aula 6 · Etapa 2: sessão e login'
    '07' = 'Aula 6 · Etapa 3: proteção da administração'
    '08' = 'Aula 6 · Etapa 4: todas as respostas de acesso'
    '09' = 'Aula 7 · Etapa 1: projeto React'
    '10' = 'Aula 7 · Etapa 2: acesso à API'
    '11' = 'Aula 7 · Etapa 3: layout e rotas'
    '12' = 'Aula 7 · Etapa 4: catálogo'
    '13' = 'Aula 7 · Etapa 5: detalhe do jogo'
}

foreach ($etapa in $etapas) {
    Write-Host "`n== $($titulos[$etapa.Pasta.Substring(0, 2)]) ==" -ForegroundColor Cyan
    Confirmar-Etapa $etapa
}

# --- Sobras -------------------------------------------------------------------
$sobras = & git.exe status --porcelain
if ($sobras) {
    Write-Host "`nATENÇÃO: há alterações fora dos commits (confira com git status):" -ForegroundColor Yellow
    $sobras | ForEach-Object { Write-Host "  $_" }
}

if ($Push) {
    Write-Host "`n== git push ==" -ForegroundColor Cyan
    Invoke-Git push
}

Write-Host "`nPronto. Histórico criado:" -ForegroundColor Cyan
& git.exe log --oneline -n 16
