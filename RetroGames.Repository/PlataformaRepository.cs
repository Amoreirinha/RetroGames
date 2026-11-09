using System.Collections.Generic;
using System.Threading.Tasks;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IPlataformaRepository
{
    Task<List<Plataforma>> ListarAsync();
    Task<Plataforma?> BuscarPorIdAsync(int id);
    Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null);
    Task<bool> PossuiJogosAsync(int id);
    Task<int> InserirAsync(Plataforma plataforma);
    Task AlterarAsync(Plataforma plataforma);
    Task ExcluirAsync(int id);
}

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    public PlataformaRepository(RepositoryConfig config) : base(config) { }

    public async Task<List<Plataforma>> ListarAsync()
    {
        var lista = new List<Plataforma>();
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma ORDER BY no_plataforma;", conexao);
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
        {
            lista.Add(new Plataforma
            {
                Id = dr.GetInt32("co_seq_plataforma"),
                Nome = dr.GetString("no_plataforma"),
                Fabricante = dr.GetString("no_fabricante"),
                AnoLancamento = dr.IsDBNull(dr.GetOrdinal("nu_ano_lancamento")) ? null : dr.GetInt32("nu_ano_lancamento")
            });
        }
        return lista;
    }

    public async Task<Plataforma?> BuscarPorIdAsync(int id)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("SELECT co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento FROM tb_plataforma WHERE co_seq_plataforma = @id;", conexao);
        AdicionarParametro(cmd, "@id", id);
        await using var dr = await cmd.ExecuteReaderAsync();

        if (await dr.ReadAsync())
        {
            return new Plataforma
            {
                Id = dr.GetInt32("co_seq_plataforma"),
                Nome = dr.GetString("no_plataforma"),
                Fabricante = dr.GetString("no_fabricante"),
                AnoLancamento = dr.IsDBNull(dr.GetOrdinal("nu_ano_lancamento")) ? null : dr.GetInt32("nu_ano_lancamento")
            };
        }
        return null;
    }

    public async Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null)
    {
        await using var conexao = await AbrirConexaoAsync();
        var sql = "SELECT COUNT(*) FROM tb_plataforma WHERE no_plataforma = @nome";
        if (idIgnorar.HasValue) sql += " AND co_seq_plataforma <> @idIgnorar";

        await using var cmd = CriarComando(sql, conexao);
        AdicionarParametro(cmd, "@nome", nome);
        if (idIgnorar.HasValue) AdicionarParametro(cmd, "@idIgnorar", idIgnorar.Value);

        var count = System.Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return count > 0;
    }

    public async Task<bool> PossuiJogosAsync(int id)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("SELECT COUNT(*) FROM tb_jogo WHERE co_plataforma = @id;", conexao);
        AdicionarParametro(cmd, "@id", id);
        var count = System.Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return count > 0;
    }

    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento) VALUES (@nome, @fabricante, @ano); SELECT LAST_INSERT_ID();", conexao);
        AdicionarParametro(cmd, "@nome", plataforma.Nome);
        AdicionarParametro(cmd, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(cmd, "@ano", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(cmd);
        return System.Convert.ToInt32(cmd.LastInsertedId);
    }

    public async Task AlterarAsync(Plataforma plataforma)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("UPDATE tb_plataforma SET no_plataforma = @nome, no_fabricante = @fabricante, nu_ano_lancamento = @ano WHERE co_seq_plataforma = @id;", conexao);
        AdicionarParametro(cmd, "@id", plataforma.Id);
        AdicionarParametro(cmd, "@nome", plataforma.Nome);
        AdicionarParametro(cmd, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(cmd, "@ano", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(cmd);
    }

    public async Task ExcluirAsync(int id)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("DELETE FROM tb_plataforma WHERE co_seq_plataforma = @id;", conexao);
        AdicionarParametro(cmd, "@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}