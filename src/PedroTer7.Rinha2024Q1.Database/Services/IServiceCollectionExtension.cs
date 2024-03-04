using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using PedroTer7.Rinha2024Q1.Database.Services;

namespace PedroTer7.Rinha2024Q1.Database;

public static class IServiceCollectionExtension
{
    public static IServiceCollection RegisterDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MariaDbRinha") ?? throw new Exception("MariaDbRinha connection string not found");

        return services
            .AddMySqlDataSource(connectionString)
            .AddScoped<IDataAccessService, DataAccessService>();
    }
}
