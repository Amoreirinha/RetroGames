using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;
using RetroGames.Model.Constantes;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorEmailAsync(string email);
    Task<Usuario?> BuscarPorIdAsync(int id);
    Task<bool> ExisteAdministradorAsync();
    Task<int> InserirAsync(Usuario usuario);
}

public class UsuarioRepository : RepositoryBase, IUsuarioRepository
{
    private const string SelectUsuario =
        "SELECT u.co_seq_usuario, u.no_usuario, u.ds_email, u.ds_senha_hash, u.co_perfil, p.ds_perfil " +
        "FROM tb_usuario u INNER JOIN tb_perfil p ON p.co_seq_perfil = u.co_perfil ";

    public UsuarioRepository(RepositoryConfig config) : base(config) { }

    public async Task<Usuario?> BuscarPorEmailAsync(string email)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando(SelectUsuario + "WHERE u.ds_email = @email;", conexao);
        AdicionarParametro(cmd, "@email", email);
        return await LerUmAsync(cmd);
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando(SelectUsuario + "WHERE u.co_seq_usuario = @id;", conexao);
        AdicionarParametro(cmd, "@id", id);
        return await LerUmAsync(cmd);
    }

    public async Task<bool> ExisteAdministradorAsync()
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando(
            "SELECT COUNT(*) FROM tb_usuario u INNER JOIN tb_perfil p ON p.co_seq_perfil = u.co_perfil WHERE p.ds_perfil = @perfil;",
            conexao);
        AdicionarParametro(cmd, "@perfil", Perfis.Administrador);
        var total = System.Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return total > 0;
    }

    // O perfil é resolvido pelo nome (NomePerfil), para o Service não precisar conhecer o id em tb_perfil.
    public async Task<int> InserirAsync(Usuario usuario)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando(
            "INSERT INTO tb_usuario (no_usuario, ds_email, ds_senha_hash, co_perfil) " +
            "SELECT @nome, @email, @hash, p.co_seq_perfil FROM tb_perfil p WHERE p.ds_perfil = @perfil;",
            conexao);
        AdicionarParametro(cmd, "@nome", usuario.Nome);
        AdicionarParametro(cmd, "@email", usuario.Email);
        AdicionarParametro(cmd, "@hash", usuario.SenhaHash);
        AdicionarParametro(cmd, "@perfil", usuario.NomePerfil);

        var linhas = await ExecutarGravacaoAsync(cmd);
        if (linhas == 0)
            throw new System.InvalidOperationException($"O perfil '{usuario.NomePerfil}' não existe em tb_perfil.");

        return System.Convert.ToInt32(cmd.LastInsertedId);
    }

    private static async Task<Usuario?> LerUmAsync(MySqlCommand cmd)
    {
        await using var dr = await cmd.ExecuteReaderAsync();
        if (!await dr.ReadAsync())
            return null;

        return new Usuario
        {
            Id = dr.GetInt32("co_seq_usuario"),
            Nome = dr.GetString("no_usuario"),
            Email = dr.GetString("ds_email"),
            SenhaHash = dr.GetString("ds_senha_hash"),
            IdPerfil = dr.GetInt32("co_perfil"),
            NomePerfil = dr.GetString("ds_perfil")
        };
    }
}
