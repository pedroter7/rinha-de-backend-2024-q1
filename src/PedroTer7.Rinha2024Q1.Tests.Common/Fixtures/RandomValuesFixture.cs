using Bogus;

namespace PedroTer7.Rinha2024Q1.Tests.Common.Fixtures;

public class RandomValuesFixture
{
    private readonly Faker _faker = new();

    public int RandomAccountId => RandomInt(1, 1000000);
    public int RandomTransactionPositiveAmount => RandomInt(1, 1000000);
    public int RandomTransactionNegativeAmount => (-1) * RandomTransactionPositiveAmount;
    public int RandomPositiveBalance => RandomInt(100, 1000000);
    public string RandomDescription => _faker.Random.AlphaNumeric(RandomInt(1, 10));
    public int RandomValidLimit => RandomInt(100, 1000000);
    public int RandomInt(int min = -1000000, int max = 1000000) => _faker.Random.Int(min, max);
    public DateTime RandomPastUtcTime => _faker.Date.Recent().ToUniversalTime();
}
