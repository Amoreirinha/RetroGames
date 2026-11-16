namespace RetroGames.WebApi.ApiModels;

/// <summary>Um jogo na listagem do catálogo.</summary>
public class ListarJogoResponseApiModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; }
    public string NomePlataforma { get; set; } = string.Empty;
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<string> NomesGeneros { get; set; } = new();
}
