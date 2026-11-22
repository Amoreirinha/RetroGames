using Microsoft.AspNetCore.Mvc;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

/// <summary>Monta a resposta 400 (código 3) com os erros por campo, para o filtro e para o [ApiController].</summary>
public static class ValidacaoConfig
{
    public static BadRequestObjectResult CriarRespostaDadosInvalidos(IDictionary<string, string[]> erros)
    {
        return new BadRequestObjectResult(new RetroGamesResponseApiModel<IDictionary<string, string[]>>
        {
            ResponseCode = ResponseCodes.DADOS_INVALIDOS,
            ErrorMessage = "Dados inválidos.",
            Data = erros
        });
    }

    // JSON malformado ou tipo errado: o [ApiController] já barra antes do filtro.
    // A mensagem é fixa para não expor detalhes internos do serializador.
    public static IActionResult RespostaModelStateInvalido(ActionContext contexto)
    {
        var erros = new Dictionary<string, string[]>();

        foreach (var (campo, entrada) in contexto.ModelState)
        {
            if (entrada is null || entrada.Errors.Count == 0)
                continue;

            erros[NormalizarCampo(campo)] = new[] { "Valor inválido ou ausente." };
        }

        return CriarRespostaDadosInvalidos(erros);
    }

    // "Nome" -> "nome", "$.anoLancamento" -> "anoLancamento": os campos saem como no JSON.
    public static string NormalizarCampo(string campo)
    {
        if (campo.StartsWith("$.", StringComparison.Ordinal))
            campo = campo[2..];

        if (string.IsNullOrEmpty(campo) || campo == "$")
            return "corpo";

        return char.ToLowerInvariant(campo[0]) + campo[1..];
    }
}
