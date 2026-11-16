using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class CondicaoJogoService : ICondicaoJogoService
{
    private readonly IRepositorySet _repositorio;

    public CondicaoJogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<List<CondicaoJogo>> ListarAsync()
    {
        return _repositorio.CondicaoJogo.ListarAsync();
    }
}
