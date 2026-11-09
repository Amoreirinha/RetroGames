using System;
using System.Data;
using System.Threading.Tasks;
using MySqlConnector;

namespace RetroGames.Repository;

public abstract class RepositoryBase
{
    private readonly RepositoryConfig _config;

    protected RepositoryBase(RepositoryConfig config)
    {
        _config = config;
    }

    protected async Task<MySqlConnection> AbrirConexaoAsync()
    {
        var conexao = new MySqlConnection(_config.ConnectionString);
        await conexao.OpenAsync();
        return conexao;
    }

    protected MySqlCommand CriarComando(string sql, MySqlConnection conexao)
    {
        var comando = conexao.CreateCommand();
        comando.CommandText = sql;
        return comando;
    }

    protected void AdicionarParametro(MySqlCommand comando, string nome, object? valor)
    {
        comando.Parameters.AddWithValue(nome, valor ?? DBNull.Value);
    }

    protected async Task<int> ExecutarGravacaoAsync(MySqlCommand comando)
    {
        try
        {
            return await comando.ExecuteNonQueryAsync();
        }
        catch (MySqlException ex) when (ex.Number == 1062)
        {
            throw new RegistroDuplicadoException("Registro duplicado encontrado no banco de dados.", ex);
        }
    }
}