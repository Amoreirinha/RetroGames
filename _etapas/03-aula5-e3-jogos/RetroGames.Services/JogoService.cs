using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Repository;

namespace RetroGames.Services;

public class JogoService : IJogoService
{
    private readonly IRepositorySet _repositorio;

    public JogoService(IRepositorySet repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(FiltroJogoServiceModel filtro)
    {
        // O filtro por gênero é resolvido aqui, com LINQ: seleciona os ids dos jogos do gênero.
        // A paginação continua no SQL, dentro do Repository.
        List<int>? idsJogos = null;
        if (filtro.IdGenero.HasValue)
        {
            var vinculos = await _repositorio.JogoGenero.ListarTodosAsync();
            idsJogos = vinculos
                .Where(v => v.IdGenero == filtro.IdGenero.Value)
                .Select(v => v.IdJogo)
                .Distinct()
                .ToList();
        }

        // Nesta aula a listagem só mostra jogos ativos (inativos ficam para a aula 6).
        var (jogos, total) = await _repositorio.Jogo.ListarAsync(
            filtro.Busca,
            filtro.IdPlataforma,
            idsJogos,
            apenasAtivos: true,
            filtro.Pagina,
            filtro.TamanhoPagina);

        var detalhes = await MontarDetalhesAsync(jogos);
        return (detalhes, total);
    }

    public async Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);
        if (jogo is null)
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        var detalhes = await MontarDetalhesAsync(new List<Jogo> { jogo });
        return detalhes[0];
    }

    public async Task<JogoDetalheServiceModel> InserirAsync(Jogo jogo, List<int> idsGeneros)
    {
        jogo.Nome = jogo.Nome.Trim();
        jogo.Ativo = true;
        idsGeneros = NormalizarGeneros(idsGeneros);

        await ValidarReferenciasAsync(jogo, idsGeneros);

        // Jogo e vínculos numa transação: ou tudo é gravado, ou nada.
        using (var transacao = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            jogo.Id = await _repositorio.Jogo.InserirAsync(jogo);
            await _repositorio.JogoGenero.InserirAsync(jogo.Id, idsGeneros);
            transacao.Complete(); // só depois da última gravação
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    public async Task<JogoDetalheServiceModel> AlterarAsync(Jogo jogo, List<int> idsGeneros)
    {
        if (await _repositorio.Jogo.BuscarPorIdAsync(jogo.Id) is null)
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        jogo.Nome = jogo.Nome.Trim();
        // O "ativo" vem do cliente: o PUT com "ativo": true reativa um jogo desativado.
        idsGeneros = NormalizarGeneros(idsGeneros);

        await ValidarReferenciasAsync(jogo, idsGeneros);

        using (var transacao = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await _repositorio.Jogo.AlterarAsync(jogo);
            await _repositorio.JogoGenero.RemoverPorJogoAsync(jogo.Id);
            await _repositorio.JogoGenero.InserirAsync(jogo.Id, idsGeneros);
            transacao.Complete();
        }

        return await BuscarPorIdAsync(jogo.Id);
    }

    public async Task DesativarAsync(int id)
    {
        var jogo = await _repositorio.Jogo.BuscarPorIdAsync(id);
        if (jogo is null)
            throw new BusinessRuleException(BusinessErrors.JOGO_NAO_ENCONTRADO);

        // Jogo nunca é apagado (RN03): só é marcado como inativo.
        await _repositorio.Jogo.DesativarAsync(id);
    }

    private static List<int> NormalizarGeneros(List<int>? idsGeneros)
    {
        return (idsGeneros ?? new List<int>()).Distinct().ToList();
    }

    // RN05: plataforma, condição e gêneros precisam existir.
    private async Task ValidarReferenciasAsync(Jogo jogo, List<int> idsGeneros)
    {
        if (await _repositorio.Plataforma.BuscarPorIdAsync(jogo.IdPlataforma) is null)
            throw new BusinessRuleException(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA);

        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        if (!condicoes.Any(c => c.Id == jogo.IdCondicaoJogo))
            throw new BusinessRuleException(BusinessErrors.CONDICAO_NAO_ENCONTRADA);

        if (idsGeneros.Count > 0)
        {
            var generos = await _repositorio.Genero.ListarAsync();
            var idsExistentes = generos.Select(g => g.Id).ToHashSet();
            if (idsGeneros.Any(id => !idsExistentes.Contains(id)))
                throw new BusinessRuleException(BusinessErrors.GENERO_NAO_ENCONTRADO);
        }
    }

    // Monta o detalhe de uma lista de jogos com LINQ, sempre com 4 consultas,
    // qualquer que seja o tamanho da lista (nunca uma consulta por jogo).
    private async Task<List<JogoDetalheServiceModel>> MontarDetalhesAsync(List<Jogo> jogos)
    {
        var detalhes = new List<JogoDetalheServiceModel>();
        if (jogos.Count == 0)
            return detalhes;

        var plataformas = await _repositorio.Plataforma.ListarAsync();
        var condicoes = await _repositorio.CondicaoJogo.ListarAsync();
        var generos = await _repositorio.Genero.ListarAsync();
        var vinculos = await _repositorio.JogoGenero.ListarPorJogosAsync(jogos.Select(j => j.Id).ToList());

        var plataformaPorId = plataformas.ToDictionary(p => p.Id);
        var condicaoPorId = condicoes.ToDictionary(c => c.Id);
        var generoPorId = generos.ToDictionary(g => g.Id);
        var idsGenerosPorJogo = vinculos
            .GroupBy(v => v.IdJogo)
            .ToDictionary(g => g.Key, g => g.Select(v => v.IdGenero).ToList());

        foreach (var jogo in jogos)
        {
            var detalhe = new JogoDetalheServiceModel { Jogo = jogo };

            if (plataformaPorId.TryGetValue(jogo.IdPlataforma, out var plataforma))
                detalhe.NomePlataforma = plataforma.Nome;

            if (condicaoPorId.TryGetValue(jogo.IdCondicaoJogo, out var condicao))
                detalhe.DescricaoCondicao = condicao.Descricao;

            if (idsGenerosPorJogo.TryGetValue(jogo.Id, out var idsGeneros))
            {
                detalhe.Generos = idsGeneros
                    .Where(id => generoPorId.ContainsKey(id))
                    .Select(id => generoPorId[id])
                    .OrderBy(g => g.Nome)
                    .ToList();
            }

            detalhes.Add(detalhe);
        }

        return detalhes;
    }
}
