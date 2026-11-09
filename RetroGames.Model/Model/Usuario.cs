namespace RetroGames.Model.Model;

public class Usuario
{
    // co_seq_usuario
    public int Id { get; set; }

    // no_usuario
    public string Nome { get; set; } = string.Empty;

    // ds_email
    public string Email { get; set; } = string.Empty;

    // ds_senha_hash
    public string SenhaHash { get; set; } = string.Empty;

    // co_perfil
    public int IdPerfil { get; set; }
}