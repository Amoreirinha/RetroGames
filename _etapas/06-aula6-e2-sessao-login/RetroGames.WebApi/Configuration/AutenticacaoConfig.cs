using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Configuration;

public static class AutenticacaoConfig
{
    public const string NomeCookie = "retrogames.sessao";

    public static IServiceCollection AddAutenticacaoCookie(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(opcoes =>
            {
                opcoes.Cookie.Name = NomeCookie;
                opcoes.Cookie.HttpOnly = true;                  // o JavaScript não lê o cookie
                opcoes.Cookie.SameSite = SameSiteMode.Strict;   // não vai em requisições de outros sites (defesa contra CSRF)
                opcoes.ExpireTimeSpan = TimeSpan.FromHours(2);
                opcoes.SlidingExpiration = true;                // renova enquanto há uso

                // 401 e 403 no envelope, sem redirecionar para uma página de login.
                opcoes.Events.OnRedirectToLogin = contexto => EscreverErroAsync(
                    contexto.HttpContext, StatusCodes.Status401Unauthorized,
                    ResponseCodes.NAO_AUTENTICADO, MensagensAcesso.NAO_AUTENTICADO);

                opcoes.Events.OnRedirectToAccessDenied = contexto => EscreverErroAsync(
                    contexto.HttpContext, StatusCodes.Status403Forbidden,
                    ResponseCodes.ACESSO_NEGADO, MensagensAcesso.ACESSO_NEGADO);
            });

        return services;
    }

    private static Task EscreverErroAsync(HttpContext http, int status, int codigo, string mensagem)
    {
        http.Response.StatusCode = status;
        return http.Response.WriteAsJsonAsync(new RetroGamesResponseApiModel
        {
            ResponseCode = codigo,
            ErrorMessage = mensagem
        });
    }
}
