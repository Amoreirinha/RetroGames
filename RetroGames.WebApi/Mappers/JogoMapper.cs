using RetroGames.Services;
using RetroGames.Model.Model;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Mappers;

public static class JogoMapper
{
    private const int TamanhoPaginaMaximo = 100;

    // Converte o request no filtro do Service, garantindo página e tamanho válidos.
    public static FiltroJogoServiceModel ParaFiltro(this ListarJogoRequestApiModel request)
    {
        return new FiltroJogoServiceModel
        {
            Busca = string.IsNullOrWhiteSpace(request.Busca) ? null : request.Busca.Trim(),
            IdPlataforma = request.IdPlataforma,
            IdGenero = request.IdGenero,
            Pagina = Math.Max(request.Pagina, 1),
            TamanhoPagina = Math.Clamp(request.TamanhoPagina, 1, TamanhoPaginaMaximo)
        };
    }

    public static ListarJogoResponseApiModel ParaResponse(this JogoDetalheServiceModel detalhe)
    {
        return new ListarJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.QuantidadeEstoque,
            Ativo = detalhe.Jogo.Ativo,
            NomePlataforma = detalhe.NomePlataforma,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            NomesGeneros = detalhe.Generos.Select(g => g.Nome).ToList()
        };
    }

    public static ListarPorIdJogoResponseApiModel ParaDetalheResponse(this JogoDetalheServiceModel detalhe)
    {
        return new ListarPorIdJogoResponseApiModel
        {
            Id = detalhe.Jogo.Id,
            Nome = detalhe.Jogo.Nome,
            Descricao = detalhe.Jogo.Descricao,
            Preco = detalhe.Jogo.Preco,
            Estoque = detalhe.Jogo.QuantidadeEstoque,
            Ativo = detalhe.Jogo.Ativo,
            IdPlataforma = detalhe.Jogo.IdPlataforma,
            NomePlataforma = detalhe.NomePlataforma,
            IdCondicaoJogo = detalhe.Jogo.IdCondicaoJogo,
            DescricaoCondicao = detalhe.DescricaoCondicao,
            Generos = detalhe.Generos.Select(g => g.ParaResponse()).ToList()
        };
    }

    public static PaginaResponseApiModel<ListarJogoResponseApiModel> ParaPaginaResponse(
        this IEnumerable<JogoDetalheServiceModel> jogos, int totalItens, int pagina, int tamanhoPagina)
    {
        return new PaginaResponseApiModel<ListarJogoResponseApiModel>
        {
            Itens = jogos.Select(j => j.ParaResponse()).ToList(),
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = totalItens,
            TotalPaginas = (int)Math.Ceiling(totalItens / (double)tamanhoPagina)
        };
    }
}
