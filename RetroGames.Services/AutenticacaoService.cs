using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class AutenticacaoService : IAutenticacaoService
{
    private readonly IRepositorySet _repositorio;
    private readonly IPasswordHasher<Usuario> _hasher;

    public AutenticacaoService(IRepositorySet repositorio, IPasswordHasher<Usuario> hasher)
    {
        _repositorio = repositorio;
        _hasher = hasher;
    }

    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        // E-mail inexistente, senha errada: o resultado é o mesmo (nulo), para não revelar qual falhou.
        var usuario = await _repositorio.Usuario.BuscarPorEmailAsync(email.Trim());
        if (usuario is null)
            return null;

        var resultado = _hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha);
        if (resultado == PasswordVerificationResult.Failed)
            return null;

        usuario.SenhaHash = string.Empty; // o hash não sai do Service
        return usuario;
    }

    public async Task GarantirAdministradorInicialAsync(string? email, string? senha)
    {
        if (await _repositorio.Usuario.ExisteAdministradorAsync())
            return;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            throw new InvalidOperationException(
                "Não há administrador cadastrado. Defina AdminInicial__Email e AdminInicial__Senha no arquivo .env (veja o .env.example).");
        }

        var administrador = new Usuario
        {
            Nome = "Administrador",
            Email = email.Trim(),
            NomePerfil = Perfis.Administrador
        };
        // A senha nunca é guardada: só o hash gerado pelo PasswordHasher.
        administrador.SenhaHash = _hasher.HashPassword(administrador, senha);

        await _repositorio.Usuario.InserirAsync(administrador);
    }
}
