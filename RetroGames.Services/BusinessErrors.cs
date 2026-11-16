using System.Net;

namespace RetroGames.Services;

/// <summary>Um erro de negócio: código, mensagem e status HTTP com que a API responde.</summary>
public sealed record BusinessError(int Code, string Message, HttpStatusCode Status);

/// <summary>Catálogo de todos os erros de negócio (Espec. 5.3). Cada erro é definido uma vez só, aqui.</summary>
public static class BusinessErrors
{
    public static readonly BusinessError PLATAFORMA_NAO_ENCONTRADA =
        new(100, "Plataforma não encontrada.", HttpStatusCode.NotFound);

    public static readonly BusinessError PLATAFORMA_NOME_DUPLICADO =
        new(101, "Já existe uma plataforma com esse nome.", HttpStatusCode.Conflict);

    public static readonly BusinessError PLATAFORMA_POSSUI_JOGOS =
        new(102, "A plataforma possui jogos e não pode ser excluída.", HttpStatusCode.Conflict);

    public static readonly BusinessError JOGO_NAO_ENCONTRADO =
        new(103, "Jogo não encontrado.", HttpStatusCode.NotFound);

    public static readonly BusinessError CONDICAO_NAO_ENCONTRADA =
        new(104, "Condição do jogo não encontrada.", HttpStatusCode.NotFound);

    public static readonly BusinessError GENERO_NAO_ENCONTRADO =
        new(105, "Gênero não encontrado.", HttpStatusCode.NotFound);
}
