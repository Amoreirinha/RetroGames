namespace RetroGames.Services;

/// <summary>Quem está chamando. O Service não conhece HTTP: a WebApi implementa esta interface.</summary>
public interface IUsuarioAtualService
{
    bool EstaLogado { get; }
    int? Id { get; }
    bool EhAdministrador { get; }
}
