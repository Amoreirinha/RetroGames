using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IJogoRepository
{
    Task<(List<Jogo> Jogos, int TotalCount)> ListarAsync(string? nome = null, int? idPlataforma = null, List<int>? idsGeneros = null, bool apenasAtivos = true, int pagina = 1, int tamanhoPagina = 10);
    Task<Jogo?> BuscarPorIdAsync(int id);
    Task<int> InserirAsync(Jogo jogo);
    Task AlterarAsync(Jogo jogo);
    Task DesativarAsync(int id);
}

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig config) : base(config) { }

    public async Task<(List<Jogo> Jogos, int TotalCount)> ListarAsync(string? nome = null, int? idPlataforma = null, List<int>? idsGeneros = null, bool apenasAtivos = true, int pagina = 1, int tamanhoPagina = 10)
    {
        var jogos = new List<Jogo>();
        await using var conexao = await AbrirConexaoAsync();

        var where = new StringBuilder(" WHERE 1=1");
        if (apenasAtivos) where.Append(" AND j.st_ativo = TRUE");
        if (!string.IsNullOrWhiteSpace(nome)) where.Append(" AND j.no_jogo LIKE @nome");
        if (idPlataforma.HasValue) where.Append(" AND j.co_plataforma = @idPlataforma");
        if (idsGeneros != null && idsGeneros.Count > 0)
        {
            where.Append($" AND j.co_seq_jogo IN (SELECT co_seq_jogo FROM tb_jogo_genero WHERE co_seq_genero IN ({string.Join(",", idsGeneros)}))");
        }

        // Contagem Total
        var sqlCount = "SELECT COUNT(*) FROM tb_jogo j" + where.ToString();
        await using var cmdCount = CriarComando(sqlCount, conexao);
        if (!string.IsNullOrWhiteSpace(nome)) AdicionarParametro(cmdCount, "@nome", $"%{nome}%");
        if (idPlataforma.HasValue) AdicionarParametro(cmdCount, "@idPlataforma", idPlataforma.Value);
        var total = System.Convert.ToInt32(await cmdCount.ExecuteScalarAsync());

        // Consulta Paginada
        var offset = (pagina - 1) * tamanhoPagina;
        var sql = $"SELECT j.co_seq_jogo, j.no_jogo, j.ds_jogo, j.vl_preco, j.qt_estoque, j.st_ativo, j.co_plataforma, j.co_condicao_jogo FROM tb_jogo j{where.ToString()} ORDER BY j.no_jogo LIMIT @limit OFFSET @offset;";
        
        await using var cmd = CriarComando(sql, conexao);
        if (!string.IsNullOrWhiteSpace(nome)) AdicionarParametro(cmd, "@nome", $"%{nome}%");
        if (idPlataforma.HasValue) AdicionarParametro(cmd, "@idPlataforma", idPlataforma.Value);
        AdicionarParametro(cmd, "@limit", tamanhoPagina);
        AdicionarParametro(cmd, "@offset", offset);

        await using var dr = await cmd.ExecuteReaderAsync();
        while (await dr.ReadAsync())
        {
            jogos.Add(new Jogo
            {
                Id = dr.GetInt32("co_seq_jogo"),
                Nome = dr.GetString("no_jogo"),
                Descricao = dr.IsDBNull(dr.GetOrdinal("ds_jogo")) ? null : dr.GetString("ds_jogo"),
                Preco = dr.GetDecimal("vl_preco"),
                QuantidadeEstoque = dr.GetInt32("qt_estoque"),
                Ativo = dr.GetBoolean("st_ativo"),
                IdPlataforma = dr.GetInt32("co_plataforma"),
                IdCondicaoJogo = dr.GetInt32("co_condicao_jogo")
            });
        }

        return (jogos, total);
    }

    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("SELECT co_seq_jogo, no_jogo, ds_jogo, vl_preco, qt_estoque, st_ativo, co_plataforma, co_condicao_jogo FROM tb_jogo WHERE co_seq_jogo = @id;", conexao);
        AdicionarParametro(cmd, "@id", id);
        await using var dr = await cmd.ExecuteReaderAsync();

        if (await dr.ReadAsync())
        {
            return new Jogo
            {
                Id = dr.GetInt32("co_seq_jogo"),
                Nome = dr.GetString("no_jogo"),
                Descricao = dr.IsDBNull(dr.GetOrdinal("ds_jogo")) ? null : dr.GetString("ds_jogo"),
                Preco = dr.GetDecimal("vl_preco"),
                QuantidadeEstoque = dr.GetInt32("qt_estoque"),
                Ativo = dr.GetBoolean("st_ativo"),
                IdPlataforma = dr.GetInt32("co_plataforma"),
                IdCondicaoJogo = dr.GetInt32("co_condicao_jogo")
            };
        }
        return null;
    }

    public async Task<int> InserirAsync(Jogo jogo)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("INSERT INTO tb_jogo (no_jogo, ds_jogo, vl_preco, qt_estoque, st_ativo, co_plataforma, co_condicao_jogo) VALUES (@nome, @descricao, @preco, @estoque, @ativo, @idPlataforma, @idCondicao); SELECT LAST_INSERT_ID();", conexao);
        AdicionarParametro(cmd, "@nome", jogo.Nome);
        AdicionarParametro(cmd, "@descricao", jogo.Descricao);
        AdicionarParametro(cmd, "@preco", jogo.Preco);
        AdicionarParametro(cmd, "@estoque", jogo.QuantidadeEstoque);
        AdicionarParametro(cmd, "@ativo", jogo.Ativo);
        AdicionarParametro(cmd, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(cmd, "@idCondicao", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(cmd);
        return System.Convert.ToInt32(cmd.LastInsertedId);
    }

    public async Task AlterarAsync(Jogo jogo)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("UPDATE tb_jogo SET no_jogo = @nome, ds_jogo = @descricao, vl_preco = @preco, qt_estoque = @estoque, st_ativo = @ativo, co_plataforma = @idPlataforma, co_condicao_jogo = @idCondicao WHERE co_seq_jogo = @id;", conexao);
        AdicionarParametro(cmd, "@id", jogo.Id);
        AdicionarParametro(cmd, "@nome", jogo.Nome);
        AdicionarParametro(cmd, "@descricao", jogo.Descricao);
        AdicionarParametro(cmd, "@preco", jogo.Preco);
        AdicionarParametro(cmd, "@estoque", jogo.QuantidadeEstoque);
        AdicionarParametro(cmd, "@ativo", jogo.Ativo);
        AdicionarParametro(cmd, "@idPlataforma", jogo.IdPlataforma);
        AdicionarParametro(cmd, "@idCondicao", jogo.IdCondicaoJogo);

        await ExecutarGravacaoAsync(cmd);
    }

    public async Task DesativarAsync(int id)
    {
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("UPDATE tb_jogo SET st_ativo = FALSE WHERE co_seq_jogo = @id;", conexao);
        AdicionarParametro(cmd, "@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}