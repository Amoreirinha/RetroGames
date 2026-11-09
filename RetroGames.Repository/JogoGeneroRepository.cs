using System.Collections.Generic;
using System.Threading.Tasks;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoGeneroRepository
{
    Task<List<JogoGenero>> ListarTodosAsync();
    Task<List<JogoGenero>> ListarPorJogosAsync(List<int> idsJogos);
    Task InserirAsync(int idJogo, List<int> idsGeneros);
    Task RemoverPorJogoAsync(int idJogo);
}

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config) : base(config) { }

    public async Task<List<JogoGenero>> ListarTodosAsync()
    {
        var lista = new List<JogoGenero>();
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("SELECT co_seq_jogo, co_seq_genero FROM tb_jogo_genero;", conexao);
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
        {
            lista.Add(new JogoGenero
            {
                IdJogo = dr.GetInt32("co_seq_jogo"),
                IdGenero = dr.GetInt32("co_seq_genero")
            });
        }
        return lista;
    }

    public async Task<List<JogoGenero>> ListarPorJogosAsync(List<int> idsJogos)
    {
        var lista = new List<JogoGenero>();
        if (idsJogos == null || idsJogos.Count == 0) return lista;

        await using var conexao = await AbrirConexaoAsync();
        var inClause = string.Join(",", idsJogos);
        await using var cmd = CriarComando($"SELECT co_seq_jogo, co_seq_genero FROM tb_jogo_genero WHERE co_seq_jogo IN ({inClause});", conexao);
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
        {
            lista.Add(new JogoGenero
            {
                IdJogo = dr.GetInt32("co_seq_jogo"),
                IdGenero = dr.GetInt32("co_seq_genero")
            });
        }
        return lista;
    }

    public async Task InserirAsync(int idJogo, List<int> idsGeneros)
    {
        if (idsGeneros == null || idsGeneros.Count == 0) return;

        await using var conexao = await AbrirConexaoAsync();
        foreach (var idGenero in idsGeneros)
        {
            await using var cmd = CriarComando("INSERT INTO tb_jogo_genero (co_seq_jogo, co_seq_genero) VALUES (@idJogo, @idGenero);", conexao);
            AdicionarParametro(cmd, "@idJogo", idJogo);
            AdicionarParametro(cmd, "@idGenero", idGenero);
            await ExecutarGravacaoAsync(cmd);
        }
    }

    public async Task RemoverPorJogoAsync(int idJogo)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("DELETE FROM tb_jogo_genero WHERE co_seq_jogo = @idJogo;", conexao);
        AdicionarParametro(cmd, "@idJogo", idJogo);
        await cmd.ExecuteNonQueryAsync();
    }
}