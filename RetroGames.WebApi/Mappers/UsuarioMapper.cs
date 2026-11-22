using System.Security.Claims;
using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Extensions;

namespace RetroGames.WebApi.Mappers;

public static class UsuarioMapper
{
    public static UsuarioLogadoResponseApiModel ParaResponse(this Usuario usuario)
    {
        return new UsuarioLogadoResponseApiModel
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.NomePerfil
        };
    }

    // GET eu: monta a resposta a partir das claims do cookie, sem ir ao banco.
    public static UsuarioLogadoResponseApiModel ParaUsuarioLogadoResponse(this ClaimsPrincipal principal)
    {
        return new UsuarioLogadoResponseApiModel
        {
            Id = principal.ObterIdUsuario() ?? 0,
            Nome = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            Email = principal.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
            Perfil = principal.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty
        };
    }
}
