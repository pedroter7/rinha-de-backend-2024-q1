using DotNet.Testcontainers.Builders;
using PedroTer7.Rinha2024Q1.WebApi.Services;

namespace PedroTer7.Rinha2024Q1.WebApi.Tests;

public class DataServiceTests
{
    [Fact(DisplayName = "Just a test")]
    public async Task JustATest()
    {
        var container = new ContainerBuilder()
            .WithImage("mariadb:11")
            .WithPortBinding(3306, true)
            .Build();

        await container.StartAsync().ConfigureAwait(false);

        var hName = container.Hostname;
        var p = container.GetMappedPublicPort(3306);
    }
}
