using FluentValidation;
using PedroTer7.Rinha2024Q1.WebApi.Dtos;

namespace PedroTer7.Rinha2024Q1.WebApi.Validators;

public class InTransactionValidator : AbstractValidator<InTransaction>
{
    public InTransactionValidator()
    {
        RuleFor(t => t.Valor)
            .GreaterThan(0)
            .WithMessage($"Valor da transação precisa ser maior que 0");

        RuleFor(t => t.Tipo)
            .Must(tt => !string.IsNullOrEmpty(tt) && (tt.Equals("c") || tt.Equals("d")))
            .WithMessage("O tipo da transaçaõ precisa uma das opções: c para crédito; d para débito");

        RuleFor(t => t.Descricao)
            .NotNull()
            .WithMessage("A descrição é obrigatória")
            .MinimumLength(1)
            .WithMessage("A descrição da transação precisa ter no mínimo 1 caractere")
            .MaximumLength(10)
            .WithMessage("A descrição da transação precisa ter no máximo 10 caracters");
    }
}
