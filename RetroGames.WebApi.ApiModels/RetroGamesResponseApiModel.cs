namespace RetroGames.WebApi.ApiModels;

/// <summary>Envelope de toda resposta da API.</summary>
public class RetroGamesResponseApiModel
{
    public int ResponseCode { get; set; } = ResponseCodes.SUCESSO;
    public string? ErrorMessage { get; set; }
}

/// <summary>Envelope com dados.</summary>
public class RetroGamesResponseApiModel<T> : RetroGamesResponseApiModel
{
    public T? Data { get; set; }
}
