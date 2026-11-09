using System.Collections.Generic;
using System.Threading.Tasks;
using RetroGames.Model.Model;

namespace RetroGames.Repository;

public interface IGeneroRepository
{
    Task<List<Genero>> ListarAsync();
}

public class GeneroRepository : RepositoryBase, IGeneroRepository
{
    public GeneroRepository(RepositoryConfig config) : base(config) { }

    public async Task<List<Genero>> ListarAsync()
    {
        var lista = new List<Genero>();
        await using var conexao = await AbrirConexaoAsync();
        await using var cmd = CriarComando("SELECT co_seq_genero, no_genero FROM tb_genero ORDER BY no_genero;", conexao);
        await using var dr = await cmd.ExecuteReaderAsync();

        while (await dr.ReadAsync())
        {
            lista.Add(new Genero
            {
                Id = dr.GetInt32("co_seq_genero"),
                Nome = dr.GetString("no_genero")
            });
        }
        return lista;
    }
}