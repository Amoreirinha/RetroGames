namespace RetroGames.Model.Model;

public class Jogo
{
    // co_seq_jogo
    public int Id { get; set; }

    // no_jogo
    public string Nome { get; set; } = string.Empty;

    // ds_jogo
    public string? Descricao { get; set; }

    // vl_preco
    public decimal Preco { get; set; }

    // qt_estoque
    public int QuantidadeEstoque { get; set; }

    // st_ativo
    public bool Ativo { get; set; }

    // co_plataforma
    public int IdPlataforma { get; set; }

    // co_condicao_jogo
    public int IdCondicaoJogo { get; set; }
}