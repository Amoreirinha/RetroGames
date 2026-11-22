namespace RetroGames.WebApi.ApiModels;

/// <summary>Entrada da alteração: o recurso completo. O id vem da rota, nunca do corpo.</summary>
public class AlterarPlataformaRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public int AnoLancamento { get; set; }
}
