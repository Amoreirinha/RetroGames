using RetroGames.Model.Model;

namespace RetroGames.Services;

public interface IAutenticacaoService
{
    /// <summary>Devolve o usuário, ou nulo se o e-mail não existe, a senha não confere ou o usuário está inativo.</summary>
    Task<Usuario?> AutenticarAsync(string email, string senha);

    /// <summary>Cria o administrador inicial se ainda não houver nenhum.</summary>
    Task GarantirAdministradorInicialAsync(string? email, string? senha);
}
