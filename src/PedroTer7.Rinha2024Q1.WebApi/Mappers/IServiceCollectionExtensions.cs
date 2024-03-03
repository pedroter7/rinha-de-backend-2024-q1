namespace PedroTer7.Rinha2024Q1.WebApi.Mappers;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMappingProfiles(this IServiceCollection services)
    {
        return services.AddAutoMapper(typeof(InToDataDtoMappingProfile))
            .AddAutoMapper(typeof(DataToOutDtoMappingProfile))
            .AddAutoMapper(typeof(ProcedureResultToDataDtoMappingProfile));
    }
}
