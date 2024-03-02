using FluentValidation;
using PedroTer7.Rinha2024Q1.WebApi.Dtos;

namespace PedroTer7.Rinha2024Q1.WebApi.Validators;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddFluentValidators(this IServiceCollection services)
    {
        return services.AddScoped<IValidator<InTransaction>, InTransactionValidator>();
    }
}
