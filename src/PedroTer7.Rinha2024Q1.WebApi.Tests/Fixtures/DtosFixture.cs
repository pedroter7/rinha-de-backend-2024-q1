using PedroTer7.Rinha2024Q1.Database.Models;
using PedroTer7.Rinha2024Q1.Tests.Common.Fixtures;

namespace PedroTer7.Rinha2024Q1.WebApi.Tests;

public class DtosFixture
{
    private readonly RandomValuesFixture _randomValuesFixture = new();

    public IEnumerable<AccountTransactionLogModel> GetRandomTransactionLogModels(int nLogs, int accountId)
    {
        int nextId = _randomValuesFixture.RandomInt(1, 100000);
        foreach (var _ in Enumerable.Range(1, nLogs))
        {
            char type = (char)_randomValuesFixture.RandomInt(99, 100);
            yield return new AccountTransactionLogModel
            {
                Account_Id = accountId,
                Amount = type == 'c' ? _randomValuesFixture.RandomTransactionPositiveAmount : _randomValuesFixture.RandomTransactionNegativeAmount,
                Description = _randomValuesFixture.RandomDescription,
                Id = nextId++,
                Timestamp_utc = _randomValuesFixture.RandomPastUtcTime,
                Type = (short)type
            };
        }
    }
}
