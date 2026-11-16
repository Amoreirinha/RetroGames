namespace RetroGames.Services;

/// <summary>Filtros e paginação da listagem de jogos.</summary>
public class FiltroJogoServiceModel
{
    public string? Busca { get; set; }
    public int? IdPlataforma { get; set; }
    public int? IdGenero { get; set; }
    public int Pagina { get; set; } = 1;
    public int TamanhoPagina { get; set; } = 12;
}
