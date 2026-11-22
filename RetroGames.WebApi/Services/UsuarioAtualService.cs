using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Services;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Services;

/// <summary>Lê o usuário logado do cookie da requisição atual.</summary>
public class UsuarioAtualService : IUsuarioAtualService
{
    private readonly IHttpContextAccessor _acessor;

    public UsuarioAtualService(IHttpContextAccessor acessor)
    {
        _acessor = acessor;
    }

    private ClaimsPrincipal? Usuario => _acessor.HttpContext?.User;

    public bool EstaLogado => Usuario?.Identity?.IsAuthenticated == true;

    public int? Id => EstaLogado ? Usuario!.ObterIdUsuario() : null;

    public bool EhAdministrador => EstaLogado && Usuario!.IsInRole(Perfis.Administrador);
}
