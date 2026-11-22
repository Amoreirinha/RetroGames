namespace RetroGames.WebApi.ApiModels;

/// <summary>Entrada da alteração: o recurso completo. "ativo": true reativa um jogo desativado.</summary>
public class AlterarJogoRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public int IdPlataforma { get; set; }
    public int IdCondicaoJogo { get; set; }
    public List<int> IdsGeneros { get; set; } = new();
    public bool Ativo { get; set; } = true;
}
