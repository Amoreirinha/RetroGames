namespace RetroGames.WebApi.ApiModels;

/// <summary>Entrada: só o que o cliente pode enviar (sem id), contra overposting.</summary>
public class InserirPlataformaRequestApiModel
{
    public string Nome { get; set; } = string.Empty;
    public string Fabricante { get; set; } = string.Empty;
    public int AnoLancamento { get; set; }
}
