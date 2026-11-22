namespace RetroGames.WebApi.ApiModels;

/// <summary>Parâmetros da listagem de jogos, lidos da query string.</summary>
public class ListarJogoRequestApiModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;

    /// <summary>Só o administrador vê jogos inativos; para os demais o parâmetro não tem efeito.</summary>
    public bool IncluirInativos { get; set; }
}
