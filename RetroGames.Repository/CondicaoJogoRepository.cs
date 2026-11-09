using System.Collections.Generic;
using System.Threading.Tasks;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface ICondicaoJogoRepository
{
    Task<List<CondicaoJogo>> ListarAsync();
}

public class CondicaoJogoRepository : RepositoryBase, ICondicaoJogoRepository
{
    public CondicaoJogoRepository(RepositoryConfig config) : base(config) { }

    public async Task<List<CondicaoJogo>> ListarAsync()
    {
        var lista = new List<CondicaoJogo>();
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("SELECT co_seq_condicao_jogo, ds_condicao_jogo FROM tb_condicao_jogo;", conexao);
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
        {
            lista.Add(new CondicaoJogo
            {
                Id = dr.GetInt32("co_seq_condicao_jogo"),
                Descricao = dr.GetString("ds_condicao_jogo")
            });
        }
        return lista;
    }
}