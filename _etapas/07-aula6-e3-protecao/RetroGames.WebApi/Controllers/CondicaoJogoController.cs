using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetroGames.Services;
using RetroGames.WebApi.ApiModels;
using RetroGames.WebApi.Mappers;

namespace RetroGames.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CondicaoJogoController : ControllerBase
{
    private readonly ICondicaoJogoService _service;

    public CondicaoJogoController(ICondicaoJogoService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<RetroGamesResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>>> Listar()
    {
        var condicoes = await _service.ListarAsync();

        return Ok(new RetroGamesResponseApiModel<List<ListarCondicaoJogoResponseApiModel>>
        {
            Data = condicoes.Select(c => c.ParaResponse()).ToList()
        });
    }
}
