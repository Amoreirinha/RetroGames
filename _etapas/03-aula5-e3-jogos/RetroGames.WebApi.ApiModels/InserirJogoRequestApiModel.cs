namespace RetroGames.WebApi.ApiModels;

/// <summary>Entrada: sem id e sem "ativo" (o jogo nasce ativo), contra overposting.</summary>
public class InserirJogoRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int IdPlataforma { get; set; }
    public int IdCondicaoJogo { get; set; }
    public List<int> IdsGeneros { get; set; } = new();
}
