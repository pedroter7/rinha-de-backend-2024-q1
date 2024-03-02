using PedroTer7.Rinha2024Q1.Database.Tests.Fixtures;

namespace PedroTer7.Rinha2024Q1.Database.Tests.Builders;

public class GetAccountStatementProcedureDynamicParametersDirector
{
    private readonly RandomValuesFixture _fixture = new();

    public DynamicParameters BuildWithRandomValues()
    {
        return new GetAccountStatementProcedureDynamicParametersBuilder()
            .WithInAccountId(_fixture.RandomAccountId)
            .Build();
    }

    public DynamicParameters BuildFor(int accountId)
    {
        return new GetAccountStatementProcedureDynamicParametersBuilder()
            .WithInAccountId(accountId)
            .Build();
    }
}
