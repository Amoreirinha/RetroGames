namespace RetroGames.Repository;

public interface IRepositorySet
{
    IPlataformaRepository Plataforma { get; }
    IJogoRepository Jogo { get; }
    IJogoGeneroRepository JogoGenero { get; }
    IGeneroRepository Genero { get; }
    ICondicaoJogoRepository CondicaoJogo { get; }
}

public class RepositorySet : IRepositorySet
{
    public IPlataformaRepository Plataforma { get; }
    public IJogoRepository Jogo { get; }
    public IJogoGeneroRepository JogoGenero { get; }
    public IGeneroRepository Genero { get; }
    public ICondicaoJogoRepository CondicaoJogo { get; }

    public RepositorySet(RepositoryConfig config)
    {
        Plataforma = new PlataformaRepository(config);
        Jogo = new JogoRepository(config);
        JogoGenero = new JogoGeneroRepository(config);
        Genero = new GeneroRepository(config);
        CondicaoJogo = new CondicaoJogoRepository(config);
    }
}