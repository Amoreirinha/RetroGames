using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(FiltroJogoServiceModel filtro);
    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);
    Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, List<int> idsGeneros);
    Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, List<int> idsGeneros);
    Task DesativarAsync(int id);
}
