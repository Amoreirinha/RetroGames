using Microsoft.AspNetCore.Diagnostics;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Middlewares;

/// <summary>Único ponto que transforma exceções não tratadas em resposta HTTP com o envelope de erro.</summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        int status;
        RetroGamesResponseApiModel resposta;

        if (exception is BusinessRuleException regra)
        {
            status = (int)regra.Erro.Status;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = regra.Erro.Code,
                ErrorMessage = regra.Message
            };
        }
        else
        {
            // O erro completo vai só para o log; o cliente recebe uma mensagem genérica.
            _logger.LogError(exception, "Erro inesperado em {Metodo} {Caminho}",
                httpContext.Request.Method, httpContext.Request.Path);

            status = StatusCodes.Status500InternalServerError;
            resposta = new RetroGamesResponseApiModel
            {
                ResponseCode = ResponseCodes.ERRO,
                ErrorMessage = "Ocorreu um erro inesperado. Tente novamente mais tarde."
            };
        }

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(resposta, cancellationToken);
        return true;
    }
}
