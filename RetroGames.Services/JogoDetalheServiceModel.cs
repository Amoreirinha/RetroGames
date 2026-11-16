using RetroGames.Model.Model;

namespace RetroGames.Services;

/// <summary>Um jogo já combinado com o nome da plataforma, a descrição da condição e os gêneros.</summary>
public class JogoDetalheServiceModel
{
    public Jogo Jogo { get; set; } = new();
    public string NomePlataforma { get; set; } = string.Empty;
    public string DescricaoCondicao { get; set; } = string.Empty;
    public List<Genero> Generos { get; set; } = new();
}
