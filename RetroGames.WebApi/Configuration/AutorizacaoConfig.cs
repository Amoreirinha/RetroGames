using Microsoft.AspNetCore.Authorization;
using RetroGames.Model.Constantes;

namespace RetroGames.WebApi.Configuration;

public static class AutorizacaoConfig
{
    public static IServiceCollection AddAutorizacao(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            // Seguro por padrão: toda rota exige login, exceto as marcadas com [AllowAnonymous].
            .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            // Sem login: 401. Logado sem o perfil: 403.
            .AddPolicy(Politicas.Administrador, politica =>
                politica.RequireAuthenticatedUser().RequireRole(Perfis.Administrador));

        return services;
    }
}
