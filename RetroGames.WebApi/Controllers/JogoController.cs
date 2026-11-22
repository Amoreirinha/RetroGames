using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JogoController : ControllerBase
{
    private readonly IJogoService _service;

    public JogoController(IJogoService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>>> Listar(
        [FromQuery] ListarJogoRequestApiModel request)
    {
        var filtro = request.ParaFiltro();
        var (jogos, total) = await _service.ListarAsync(filtro);

        return Ok(new RetroGamesResponseApiModel<PaginaResponseApiModel<ListarJogoResponseApiModel>>
        {
            Data = jogos.ParaPaginaResponse(total, filtro.Pagina, filtro.TamanhoPagina)
        });
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> ListarPorId(int id)
    {
        var detalhe = await _service.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel> { Data = detalhe.ParaDetalheResponse() });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Inserir(InserirJogoRequestApiModel request)
    {
        var criado = await _service.InserirAsync(request.ParaModel(), request.IdsGeneros);

        return CreatedAtAction(nameof(ListarPorId), new { id = criado.Jogo.Id },
            new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel> { Data = criado.ParaDetalheResponse() });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel>>> Alterar(int id, AlterarJogoRequestApiModel request)
    {
        var alterado = await _service.AlterarAsync(request.ParaModel(id), request.IdsGeneros);

        return Ok(new RetroGamesResponseApiModel<ListarPorIdJogoResponseApiModel> { Data = alterado.ParaDetalheResponse() });
    }

    [Authorize(Policy = Politicas.Administrador)]
    // Exclusão lógica (RN03): o jogo nunca é apagado, só desativado.
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Desativar(int id)
    {
        await _service.DesativarAsync(id);

        return Ok(new RetroGamesResponseApiModel());
    }
}
