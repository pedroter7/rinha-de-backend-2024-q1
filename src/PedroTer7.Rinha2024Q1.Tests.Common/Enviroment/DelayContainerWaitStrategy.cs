using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace PedroTer7.Rinha2024Q1.Tests.Common.Enviroment;

public class DelayContainerWaitStrategy(int delayMs) : IWaitUntil
{
    private readonly int _delayMs = delayMs;

    public async Task<bool> UntilAsync(IContainer container)
    {
        await Task.Delay(_delayMs);
        return true;
    }
}
