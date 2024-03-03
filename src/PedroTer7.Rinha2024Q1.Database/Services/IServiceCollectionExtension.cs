using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using PedroTer7.Rinha2024Q1.Database.Services;

namespace PedroTer7.Rinha2024Q1.Database;

public static class IServiceCollectionExtension
{
    public static IServiceCollection RegisterDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var readConnString = configuration.GetConnectionString("MariaDbRead") ?? throw new Exception("mariadb-read connection string not found");
        var writeConnString = configuration.GetConnectionString("MariaDbWrite") ?? throw new Exception("mariadb-write connection string not found");

        return services
            .AddKeyedMySqlDataSource("read", readConnString)
            .AddKeyedMySqlDataSource("write", writeConnString)
            .AddScoped<IDataAccessService, DataAccessService>();
    }
}
