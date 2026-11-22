using System.Globalization;
using System.Security.Claims;
using RetroGames.Model.Model;

namespace RetroGames.WebApi.Extensions;

public static class ClaimsPrincipalExtensions
{
    // As claims vão, criptografadas, dentro do cookie de sessão.
    public static ClaimsPrincipal ParaClaimsPrincipal(this Usuario usuario, string esquema)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.NomePerfil)
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, esquema));
    }

    public static int? ObterIdUsuario(this ClaimsPrincipal principal)
    {
        var valor = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) ? id : null;
    }
}
