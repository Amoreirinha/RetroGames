namespace RetroGames.Services;

/// <summary>Lançada quando uma regra de negócio é violada. Guarda o erro do catálogo.</summary>
public class BusinessRuleException : Exception
{
    public BusinessError Erro { get; }

    public BusinessRuleException(BusinessError erro) : base(erro.Message)
    {
        Erro = erro;
    }
}
