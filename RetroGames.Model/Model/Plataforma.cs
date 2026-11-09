namespace RetroGames.Model.Model;

public class Plataforma
{
    // co_seq_plataforma
    public int Id { get; set; }

    // no_plataforma
    public string Nome { get; set; } = string.Empty;

    // no_fabricante
    public string Fabricante { get; set; } = string.Empty;

    // nu_ano_lancamento
    public int? AnoLancamento { get; set; }
}