using PedroTer7.Rinha2024Q1.Database.Tests.Builders;
using PedroTer7.Rinha2024Q1.Tests.Common.Enviroment;
using PedroTer7.Rinha2024Q1.Tests.Common.Fixtures;

namespace PedroTer7.Rinha2024Q1.Database.Tests;

[Collection("DatabaseTests")]
public class TransactionProcedureTests(TransactionProcedureDynamicParametersDirector director, RandomValuesFixture fixture)
    : IAsyncLifetime, IClassFixture<TransactionProcedureDynamicParametersDirector>, IClassFixture<RandomValuesFixture>
{
    private readonly MariaDbTestEnvironment _testEnvironment = new();
    private readonly TransactionProcedureDynamicParametersDirector _director = director;
    private readonly RandomValuesFixture _fixture = fixture;

    public Task InitializeAsync()
    {
        return _testEnvironment.Container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _testEnvironment.Container.DisposeAsync().AsTask();
    }

    [Fact(DisplayName = "Procedure 'transaction' returns correctly for nonexistent account for 'c' operation")]
    [Trait("procedure", "transaction")]
    public async Task TransactionProcedure_ReturnsCorrectly_ForNonexistentAccount_ForOperation_c()
    {
        // Arrange
        using var conn = _testEnvironment.Connection;
        char op = 'c';
        var p = _director.BuildWithRandomValidValuesFor(op);
        var (countQuery, negatedCountQuery) = BuildCountTransactionLogsSqlQueries(GetInAccountId(p), GetInDescription(p), op, GetInAmount(p));

        // Act
        var queryResult = await QueryProcedure(conn, p);

        // Assert
        Assert.Equal(1, GetOutCode(p));
        Assert.Null(GetOutLimit(p));
        Assert.Null(GetOutBalance(p));
        Assert.Empty(queryResult);
        Assert.Equal(0L, await conn.ExecuteScalarAsync(countQuery));
        Assert.Equal(0L, await conn.ExecuteScalarAsync(negatedCountQuery));
    }

    [Fact(DisplayName = "Procedure 'transaction' returns correctly for nonexistent account for 'd' operation")]
    [Trait("procedure", "transaction")]
    public async Task TransactionProcedure_ReturnsCorrectly_ForNonexistentAccount_ForOperation_d()
    {
        // Arrange
        using var conn = _testEnvironment.Connection;
        char op = 'd';
        var p = _director.BuildWithRandomValidValuesFor(op);
        var (countQuery, negatedCountQuery) = BuildCountTransactionLogsSqlQueries(GetInAccountId(p), GetInDescription(p), op, GetInAmount(p));

        // Act
        var queryResult = await QueryProcedure(conn, p);

        // Assert
        Assert.Equal(1, GetOutCode(p));
        Assert.Null(GetOutLimit(p));
        Assert.Null(GetOutBalance(p));
        Assert.Empty(queryResult);
        Assert.Equal(0L, await conn.ExecuteScalarAsync(countQuery));
        Assert.Equal(0L, await conn.ExecuteScalarAsync(negatedCountQuery));
    }

    [Fact(DisplayName = "Procedure 'transaction' returns correctly for invalid input combinations for operation 'c'")]
    [Trait("procedure", "transaction")]
    public async Task TransactionProcedure_ReturnsCorrectly_ForInvalidInputCombinations_ForOperation_c()
    {
        // Arrange
        using var conn = _testEnvironment.Connection;
        char op = 'c';
        var p = _director.BuildWithInvalidCombinationOfRandomValuesFor(op);
        var (countQuery, negatedCountQuery) = BuildCountTransactionLogsSqlQueries(GetInAccountId(p), GetInDescription(p), op, GetInAmount(p));

        // Act
        var queryResult = await QueryProcedure(conn, p);

        // Assert
        Assert.Equal(3, GetOutCode(p));
        Assert.Null(GetOutLimit(p));
        Assert.Null(GetOutBalance(p));
        Assert.Empty(queryResult);
        Assert.Equal(0L, await conn.ExecuteScalarAsync(countQuery));
        Assert.Equal(0L, await conn.ExecuteScalarAsync(negatedCountQuery));
    }

    [Fact(DisplayName = "Procedure 'transaction' returns correctly for invalid input combinations for operation 'd'")]
    [Trait("procedure", "transaction")]
    public async Task TransactionProcedure_ReturnsCorrectly_ForInvalidInputCombinations_ForOperation_d()
    {
        // Arrange
        using var conn = _testEnvironment.Connection;
        char op = 'd';
        var p = _director.BuildWithInvalidCombinationOfRandomValuesFor(op);
        var (countQuery, negatedCountQuery) = BuildCountTransactionLogsSqlQueries(GetInAccountId(p), GetInDescription(p), op, GetInAmount(p));

        // Act
        var queryResult = await QueryProcedure(conn, p);

        // Assert
        Assert.Equal(3, GetOutCode(p));
        Assert.Null(GetOutLimit(p));
        Assert.Null(GetOutBalance(p));
        Assert.Empty(queryResult);
        Assert.Equal(0L, await conn.ExecuteScalarAsync(countQuery));
        Assert.Equal(0L, await conn.ExecuteScalarAsync(negatedCountQuery));
    }

    [Fact(DisplayName = "Procedure 'transaction' performs correctly for 'c' operation")]
    [Trait("procedure", "transaction")]
    public async Task TransactionProcedure_PerformsCorrectly_ForValidInputs_ForOperation_c()
    {
        // Arrange
        using var conn = _testEnvironment.Connection;
        char op = 'c';
        var accountId = _fixture.RandomInt(1, 5);
        var transactionAmount = _fixture.RandomTransactionPositiveAmount;
        var initialBalance = _fixture.RandomInt(0, 10000);
        await conn.ExecuteAsync(InsertTestAccountsSqlQuery);
        await conn.ExecuteAsync(SqlQueriesUtil.BuildUpdateAccountBalance(accountId, initialBalance));
        var p = _director.BuildWithValidRandomValuesFor(accountId, 'c', transactionAmount);
        var (countQuery, negatedCountQuery) = BuildCountTransactionLogsSqlQueries(accountId, GetInDescription(p), op, transactionAmount);

        // Act
        var queryResult = await QueryProcedure(conn, p);

        // Assert
        Assert.Equal(0, GetOutCode(p));
        Assert.Equal(accountId * 100, GetOutLimit(p));
        Assert.Equal(transactionAmount + initialBalance, GetOutBalance(p));
        Assert.Empty(queryResult);
        Assert.Equal(1L, await conn.ExecuteScalarAsync(countQuery));
        Assert.Equal(0L, await conn.ExecuteScalarAsync(negatedCountQuery));
    }

    [Fact(DisplayName = "Procedure 'transaction' performs correctly for 'd' operation")]
    [Trait("procedure", "transaction")]
    public async Task TransactionProcedure_PerformsCorrectly_ForValidInputs_ForOperation_d()
    {
        // Arrange
        using var conn = _testEnvironment.Connection;
        char op = 'd';
        var accountId = _fixture.RandomInt(1, 5);
        var transactionAmount = _fixture.RandomTransactionNegativeAmount;
        var initialBalance = _fixture.RandomInt((-1) * transactionAmount, (-1) * transactionAmount + 1000);
        await conn.ExecuteAsync(InsertTestAccountsSqlQuery);
        await conn.ExecuteAsync(SqlQueriesUtil.BuildUpdateAccountBalance(accountId, initialBalance));
        var p = _director.BuildWithValidRandomValuesFor(accountId, 'd', transactionAmount);
        var (countQuery, negatedCountQuery) = BuildCountTransactionLogsSqlQueries(accountId, GetInDescription(p), op, transactionAmount);

        // Act
        var queryResult = await QueryProcedure(conn, p);

        // Assert
        Assert.Equal(0, GetOutCode(p));
        Assert.Equal(accountId * 100, GetOutLimit(p));
        Assert.Equal(initialBalance + transactionAmount, GetOutBalance(p));
        Assert.Empty(queryResult);
        Assert.Equal(1L, await conn.ExecuteScalarAsync(countQuery));
        Assert.Equal(0L, await conn.ExecuteScalarAsync(negatedCountQuery));
    }

    [Fact(DisplayName = "Procedure 'transaction' performs correctly for 'd' operation when there aren't enough funds")]
    [Trait("procedure", "transaction")]
    public async Task TransactionProcedure_PerformsCorrectly_ForOperation_d_WhenThereArentEnoughFunds()
    {
        // Arrange
        using var conn = _testEnvironment.Connection;
        char op = 'd';
        var accountId = _fixture.RandomInt(1, 5);
        var transactionAmount = (-1) * (accountId * 100 + 1);
        await conn.ExecuteAsync(InsertTestAccountsSqlQuery);
        var p = _director.BuildWithValidRandomValuesFor(accountId, op, transactionAmount);
        var (countQuery, negatedCountQuery) = BuildCountTransactionLogsSqlQueries(accountId, GetInDescription(p), op, transactionAmount);

        // Act
        var queryResult = await QueryProcedure(conn, p);

        // Assert
        Assert.Equal(2, GetOutCode(p));
        Assert.Null(GetOutLimit(p));
        Assert.Null(GetOutBalance(p));
        Assert.Empty(queryResult);
        Assert.Equal(0L, await conn.ExecuteScalarAsync(countQuery));
        Assert.Equal(0L, await conn.ExecuteScalarAsync(negatedCountQuery));
    }

    private static Task<IEnumerable<object>> QueryProcedure(MySqlConnection conn, DynamicParameters p)
    {
        return conn.QueryAsync<object>("transaction", p, null, commandType: CommandType.StoredProcedure);
    }

    private static string InsertTestAccountsSqlQuery => SqlQueriesUtil.BuildInsertIntoAccount(5);

    private static string BuildCountTransactionLogsSqlQuery(long accountId, string description, char operation, long amount)
        => $@"
            SELECT count(*)
            FROM account_transaction_log
            WHERE account_id = {accountId}
                AND description = '{description}'
                AND `type` = {(short)operation}
                AND amount = {amount};
        ";

    private static string BuildCountTransactionLogsNegatedSqlQuery(long accountId, string description, char operation, long amount)
        => $@"
            SELECT count(*)
            FROM account_transaction_log
            WHERE NOT (account_id = {accountId}
                AND description = '{description}'
                AND `type` = {(short)operation}
                AND amount = {amount});
        ";

    private static (string countQuery, string negatedCountQuery) BuildCountTransactionLogsSqlQueries(long accountId, string description, char operation, long amount)
        => (BuildCountTransactionLogsSqlQuery(accountId, description, operation, amount),
            BuildCountTransactionLogsNegatedSqlQuery(accountId, description, operation, amount));

    private static short GetOutCode(DynamicParameters procedureParameters) => procedureParameters.Get<short>("out_code");
    private static int? GetOutLimit(DynamicParameters procedureParameters) => procedureParameters.Get<int?>("out_limit");
    private static int? GetOutBalance(DynamicParameters procedureParameters) => procedureParameters.Get<int?>("out_balance");
    private static string GetInDescription(DynamicParameters procedureParameters) => procedureParameters.Get<string>("in_description");
    private static int GetInAccountId(DynamicParameters procedureParameters) => procedureParameters.Get<int>("in_account_id");
    private static int GetInAmount(DynamicParameters procedureParameters) => procedureParameters.Get<int>("in_amount");
}