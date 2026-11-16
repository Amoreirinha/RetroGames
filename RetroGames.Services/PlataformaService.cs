using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class PlataformaService : IPlataformaService
{
    private readonly IRepositorySet _repositorio;

    public PlataformaService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public Task<List<Plataforma>> ListarAsync()
    {
        return _repositorio.Plataforma.ListarAsync();
    }

    public async Task<Plataforma> BuscarPorIdAsync(int id)
    {
        var plataforma = await _repositorio.Plataforma.BuscarPorIdAsync(id);
        if (plataforma is null)
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);

        return plataforma;
    }

    public async Task<Plataforma> InserirAsync(Plataforma plataforma)
    {
        plataforma.Nome = plataforma.Nome.Trim();

        if (await _repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome))
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);

        try
        {
            plataforma.Id = await _repositorio.Plataforma.InserirAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            // Duas gravações ao mesmo tempo: o banco barrou a segunda.
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }

        return plataforma;
    }

    public async Task<Plataforma> AlterarAsync(Plataforma plataforma)
    {
        await BuscarPorIdAsync(plataforma.Id);

        plataforma.Nome = plataforma.Nome.Trim();

        // Ignora a própria plataforma: manter o próprio nome não é duplicidade.
        if (await _repositorio.Plataforma.ExisteComNomeAsync(plataforma.Nome, plataforma.Id))
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);

        try
        {
            await _repositorio.Plataforma.AlterarAsync(plataforma);
        }
        catch (RegistroDuplicadoException)
        {
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NOME_DUPLICADO);
        }

        return plataforma;
    }

    public async Task ExcluirAsync(int id)
    {
        await BuscarPorIdAsync(id);

        if (await _repositorio.Plataforma.PossuiJogosAsync(id))
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_POSSUI_JOGOS);

        await _repositorio.Plataforma.ExcluirAsync(id);
    }
}
