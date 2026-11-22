using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using RetroGames.WebApi.Configuration;

namespace RetroGames.WebApi.Filters;

/// <summary>
/// Roda depois do model binding e antes da action: procura um IValidator para cada argumento
/// e, havendo erro, responde 400 (código 3) sem chamar a action.
/// </summary>
public class FluentValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _servicos;

    public FluentValidationFilter(IServiceProvider servicos)
    {
        _servicos = servicos;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext contexto, ActionExecutionDelegate proximo)
    {
        var erros = new Dictionary<string, List<string>>();

        foreach (var argumento in contexto.ActionArguments.Values)
        {
            if (argumento is null)
                continue;

            var tipoDoValidator = typeof(IValidator<>).MakeGenericType(argumento.GetType());
            if (_servicos.GetService(tipoDoValidator) is not IValidator validator)
                continue;

            var resultado = await validator.ValidateAsync(
                new ValidationContext<object>(argumento), contexto.HttpContext.RequestAborted);

            foreach (var falha in resultado.Errors)
            {
                var campo = ValidacaoConfig.NormalizarCampo(falha.PropertyName);
                if (!erros.TryGetValue(campo, out var mensagens))
                {
                    mensagens = new List<string>();
                    erros[campo] = mensagens;
                }
                mensagens.Add(falha.ErrorMessage);
            }
        }

        if (erros.Count > 0)
        {
            contexto.Result = ValidacaoConfig.CriarRespostaDadosInvalidos(
                erros.ToDictionary(par => par.Key, par => par.Value.ToArray()));
            return;
        }

        await proximo();
    }
}
