using FluentValidation;
using RetroGames.WebApi.ApiModels;

namespace RetroGames.WebApi.Validators;

// Só formato. Plataforma, condição e gêneros inexistentes (RN05) são regras do JogoService.
public class InserirJogoRequestValidator : AbstractValidator<InserirJogoRequestApiModel>
{
    public InserirJogoRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Informe o nome.")
            .MaximumLength(120).WithMessage("O nome deve ter no máximo 120 caracteres.");

        RuleFor(x => x.Descricao)
            .MaximumLength(1000).WithMessage("A descrição deve ter no máximo 1000 caracteres.");

        RuleFor(x => x.Preco)
            .GreaterThan(0).WithMessage("O preço deve ser maior que zero.")
            .LessThanOrEqualTo(99999.99m).WithMessage("O preço deve ser no máximo 99.999,99.");

        RuleFor(x => x.Estoque)
            .GreaterThanOrEqualTo(0).WithMessage("O estoque não pode ser negativo.");

        RuleFor(x => x.IdPlataforma)
            .GreaterThan(0).WithMessage("Informe a plataforma.");

        RuleFor(x => x.IdCondicaoJogo)
            .GreaterThan(0).WithMessage("Informe a condição do jogo.");

        RuleFor(x => x.IdsGeneros)
            .NotNull().WithMessage("Informe a lista de gêneros.");

        RuleForEach(x => x.IdsGeneros)
            .GreaterThan(0).WithMessage("Gênero inválido.");
    }
}
