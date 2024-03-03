using PedroTer7.Rinha2024Q1.Tests.Common.Fixtures;

namespace PedroTer7.Rinha2024Q1.Database.Tests.Builders;

public class TransactionProcedureDynamicParametersDirector
{
    private readonly RandomValuesFixture _fixture = new();

    public DynamicParameters BuildWithRandomValidValuesFor(char operation)
    {
        ValidateOperation(operation);
        int inAmount = operation == 'c' ? _fixture.RandomTransactionPositiveAmount : _fixture.RandomTransactionNegativeAmount;

        return new TransactionProcedureDynamicParametersBuilder()
            .WithInAccountId(_fixture.RandomAccountId)
            .WithInAmount(inAmount)
            .WithInDescription(_fixture.RandomDescription)
            .WithInType((short)operation)
            .Build();
    }

    public DynamicParameters BuildWithInvalidCombinationOfRandomValuesFor(char operation)
    {
        ValidateOperation(operation);
        int inAmount = operation == 'c' ? _fixture.RandomTransactionNegativeAmount : _fixture.RandomTransactionPositiveAmount;

        return new TransactionProcedureDynamicParametersBuilder()
            .WithInAccountId(_fixture.RandomAccountId)
            .WithInAmount(inAmount)
            .WithInDescription(_fixture.RandomDescription)
            .WithInType((short)operation)
            .Build();
    }

    public DynamicParameters BuildWithValidRandomValuesFor(int accountId, char operation, int amount)
    {
        ValidateOperation(operation);
        ValidateAmount(operation, amount);

        return new TransactionProcedureDynamicParametersBuilder()
            .WithInAccountId(accountId)
            .WithInAmount(amount)
            .WithInDescription(_fixture.RandomDescription)
            .WithInType((short)operation)
            .Build();
    }

    private static void ValidateAmount(char operation, int amount)
    {
        if (operation == 'c' && amount < 0 || operation == 'd' && amount > 0)
            throw new ArgumentException("For op 'c' amount must be positive, for op 'd' must be negative");
    }

    private static void ValidateOperation(char operation)
    {
        if (operation != 'c' && operation != 'd')
            throw new ArgumentException("Must be 'c' or 'd'", nameof(operation));
    }
}
