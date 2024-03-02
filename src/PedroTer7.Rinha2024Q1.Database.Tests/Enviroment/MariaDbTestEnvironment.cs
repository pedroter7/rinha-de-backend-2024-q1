using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace PedroTer7.Rinha2024Q1.Database.Tests.Enviroment;

public class MariaDbTestEnvironment
{
    private static readonly string DockerImage = Environment.GetEnvironmentVariable("DOCKER_DATABASE_TEST_IMAGE")
            ?? throw new Exception("Could not get DOCKER_DATABASE_TEST_IMAGE env variable");

    private const string DbUser = "root";
    private const string DbName = "rinha_2024_q1";

    private string ConnectionString => $"Server=localhost;User Id={DbUser};Port={_container.GetMappedPublicPort(3306)};Database={DbName}";

    private readonly IContainer _container = new ContainerBuilder()
            .WithImage(DockerImage)
            .WithEnvironment("MARIADB_ALLOW_EMPTY_ROOT_PASSWORD", "1")
            .WithPortBinding(3306, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                .UntilMessageIsLogged("mariadbd: ready for connections")
                .AddCustomWaitStrategy(new DelayContainerWaitStrategy(5000)))
            .Build();

    public IContainer Container => _container;

    public MySqlConnection Connection => new(ConnectionString);
}
