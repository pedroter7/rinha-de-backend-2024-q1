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
            .MinimumLength(1)
            .MaximumLength(10)
            .WithMessage("A descrição da transação precisa ter entre 1 e 10 caracteres");
    }
}
