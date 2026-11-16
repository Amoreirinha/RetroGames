using RetroGames.Repository;
using RetroGames.Services;

namespace RetroGames.WebApi.Configuration;

public static class InjecaoDependenciaConfig
{
    public static IServiceCollection AddRepositorios(this IServiceCollection services, string connectionString)
    {
        // Config: uma única instância. Repositórios e RepositorySet: um por requisição.
        services.AddSingleton(new RepositoryConfig { ConnectionString = connectionString });

        services.AddScoped<IPlataformaRepository, PlataformaRepository>();
        services.AddScoped<IJogoRepository, JogoRepository>();
        services.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();
        services.AddScoped<IRepositorySet, RepositorySet>();

        return services;
    }

    public static IServiceCollection AddServicosDeNegocio(this IServiceCollection services)
    {
        services.AddScoped<IPlataformaService, PlataformaService>();
        services.AddScoped<IJogoService, JogoService>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();

        return services;
    }
}
