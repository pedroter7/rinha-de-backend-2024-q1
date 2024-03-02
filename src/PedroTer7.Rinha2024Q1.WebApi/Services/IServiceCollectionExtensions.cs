using PedroTer7.Rinha2024Q1.WebApi.Services;

namespace PedroTer7.Rinha2024Q1.WebApi;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        return services.AddScoped<IDataService, DataService>();
    }
}
