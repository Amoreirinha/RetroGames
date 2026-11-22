using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlataformaController : ControllerBase
{
    private readonly IPlataformaService _service;

    public PlataformaController(IPlataformaService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>>> Listar()
    {
        var plataformas = await _service.ListarAsync();

        return Ok(new RetroGamesResponseApiModel<List<ListarPlataformaResponseApiModel>>
        {
            Data = plataformas.Select(p => p.ParaResponse()).ToList()
        });
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> ListarPorId(int id)
    {
        var plataforma = await _service.BuscarPorIdAsync(id);

        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel> { Data = plataforma.ParaResponse() });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPost]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> Inserir(InserirPlataformaRequestApiModel request)
    {
        var criada = await _service.InserirAsync(request.ParaModel());

        // 201 Created, com o cabeçalho Location apontando para a action de leitura.
        return CreatedAtAction(nameof(ListarPorId), new { id = criada.Id },
            new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel> { Data = criada.ParaResponse() });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel<ListarPlataformaResponseApiModel>>> Alterar(int id, AlterarPlataformaRequestApiModel request)
    {
        var alterada = await _service.AlterarAsync(request.ParaModel(id));

        return Ok(new RetroGamesResponseApiModel<ListarPlataformaResponseApiModel> { Data = alterada.ParaResponse() });
    }

    [Authorize(Policy = Politicas.Administrador)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<RetroGamesResponseApiModel>> Excluir(int id)
    {
        await _service.ExcluirAsync(id);

        return Ok(new RetroGamesResponseApiModel());
    }
}
