namespace RetroGames.Services;

/// <summary>Filtros e paginação da listagem de jogos.</summary>
public class FiltroJogoServiceModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;

    /// <summary>Só vale para o administrador (RN04); para os demais é ignorado.</summary>
    public bool IncluirInativos { get; set; }
}
