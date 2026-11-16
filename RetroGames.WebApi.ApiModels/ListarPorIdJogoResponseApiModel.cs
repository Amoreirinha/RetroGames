namespace RetroGames.WebApi.ApiModels;

/// <summary>O detalhe de um jogo.</summary>
public class ListarPorIdJogoResponseApiModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; }
    public int IdPlataforma { get; set; }
    public string NomePlataforma { get; set; } = string.Empty;
    public int IdCondicaoJogo { get; set; }
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<ListarGeneroResponseApiModel> Generos { get; set; } = new();
}
