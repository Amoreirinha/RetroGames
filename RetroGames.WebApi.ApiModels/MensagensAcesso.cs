namespace RetroGames.WebApi.ApiModels;

/// <summary>Mensagens das respostas de acesso (401 e 403).</summary>
public static class MensagensAcesso
{
    public const string NAO_AUTENTICADO = "É necessário estar autenticado para acessar este recurso.";
    public const string ACESSO_NEGADO = "Acesso negado. Este recurso é restrito ao administrador.";
    public const string CREDENCIAIS_INVALIDAS = "E-mail ou senha inválidos.";
}
