using System.Diagnostics;
using PedroTer7.Rinha2024Q1.Database.Models;
using PedroTer7.Rinha2024Q1.Database.Tests.Builders;
using PedroTer7.Rinha2024Q1.Database.Tests.Enviroment;
using PedroTer7.Rinha2024Q1.Database.Tests.Fixtures;

namespace PedroTer7.Rinha2024Q1.Database.Tests;

public class GetAccountStatementProcedureTests(GetAccountStatementProcedureDynamicParametersDirector director,
    RandomValuesFixture fixture)
    : IAsyncLifetime, IClassFixture<GetAccountStatementProcedureDynamicParametersDirector>, IClassFixture<RandomValuesFixture>
{
    private const long AcceptableTimeDiffMs = 1000 * 60;
    private readonly MariaDbTestEnvironment _enviroment = new();
    private readonly GetAccountStatementProcedureDynamicParametersDirector _director = director;
    private readonly RandomValuesFixture _fixture = fixture;

    public Task InitializeAsync()
    {
        return _enviroment.Container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _enviroment.Container.DisposeAsync().AsTask();
    }

    [Fact(DisplayName = "Returns correctly for nonexistent account")]
    [Trait("procedure", "get_account_statement")]
    public async Task GetAccountStatementProcedure_ReturnsCorretly_ForNonExistentAccount()
    {
        // Arrange
        using var conn = _enviroment.Connection;
        DynamicParameters p = _director.BuildWithRandomValues();
        var timeCalled = DateTime.UtcNow;

        // Act
        var result = await QueryProcedure(conn, p);

        // Assert
        Assert.Equal(1, GetOutCode(p));
        Assert.Null(GetOutCurrentBalance(p));
        Assert.Null(GetOutCurrentLimit(p));
        Assert.True((timeCalled - GetOutStatementTimeStamp(p).ToUniversalTime()).Milliseconds < AcceptableTimeDiffMs);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "Returns correctly for existent account with no transaction history")]
    [Trait("procedure", "get_account_statement")]
    public async Task GetAccountStatementProcedure_ReturnsCorretly_ForExistentAccount_WithNoTransactionHistory()
    {
        // Arrange
        using var conn = _enviroment.Connection;
        int balance = _fixture.RandomPositiveBalance;
        int nAccounts = _fixture.RandomInt(1, 10);
        int accountId = _fixture.RandomInt(1, nAccounts);
        int x = _fixture.RandomInt(1, 1000);
        await conn.ExecuteAsync(SqlQueriesUtil.BuildInsertIntoAccount(nAccounts, i => i * x));
        await conn.ExecuteAsync(SqlQueriesUtil.BuildUpdateAccountBalanceCache(accountId, balance));
        DynamicParameters p = _director.BuildFor(accountId);
        var timeCalled = DateTime.UtcNow;

        // Act
        var result = await QueryProcedure(conn, p);

        // Assert
        Assert.Equal(0, GetOutCode(p));
        Assert.Equal(balance, GetOutCurrentBalance(p));
        Assert.Equal(accountId * x, GetOutCurrentLimit(p));
        Assert.True((timeCalled - GetOutStatementTimeStamp(p).ToUniversalTime()).Milliseconds < AcceptableTimeDiffMs);
        Assert.Empty(result);
    }

    [Fact(DisplayName = "Returns transaction history correctly for existent account")]
    [Trait("procedure", "get_account_statement")]
    public async Task GetAccountStatementProcedure_ReturnsTransactionHistoryCorretly_ForExistentAccount()
    {
        // Arrange
        using var conn = _enviroment.Connection;
        int nAccounts = _fixture.RandomInt(1, 10);
        int accountId = _fixture.RandomInt(1, nAccounts);
        int nLogsForTestedAccount = _fixture.RandomInt(1, 15);
        int x = _fixture.RandomInt(1, 1000);
        await conn.ExecuteAsync(SqlQueriesUtil.BuildInsertIntoAccount(nAccounts, i => i * x));
        await conn.ExecuteAsync(SqlQueriesUtil.BuildUpdateAccountBalanceCache(accountId, _fixture.RandomPositiveBalance));
        foreach (var id in Enumerable.Range(1, nAccounts))
        {
            var logsN = id == accountId ? nLogsForTestedAccount : _fixture.RandomInt(1, 15);
            await conn.ExecuteAsync(SqlQueriesUtil.BuildInsertIntoTransactionLogs(id, logsN));
        }

        var testedAccountLast10Transactions =
            (await conn.QueryAsync<AccountTransactionLogModel>(SqlQueriesUtil.BuildSelectTransactionLogsForAccount(accountId)))
            .OrderByDescending(x => x.Timestamp_utc)
            .Take(10);
        DynamicParameters p = _director.BuildFor(accountId);
        var timeCalled = DateTime.UtcNow;

        // Act
        var result = await QueryProcedure(conn, p);

        // Assert
        Assert.Equal(0, GetOutCode(p));
        Assert.True((timeCalled - GetOutStatementTimeStamp(p)).Milliseconds < AcceptableTimeDiffMs);
        Assert.True(result.Any());
        Assert.True(nLogsForTestedAccount < 10 ? result.Count() == nLogsForTestedAccount : result.Count() == 10);
        Assert.True(result.All(r => r.Account_Id == accountId));
        Assert.Equal(testedAccountLast10Transactions, result);
    }

    private static Task<IEnumerable<AccountTransactionLogModel>> QueryProcedure(MySqlConnection conn, DynamicParameters p)
    {
        return conn.QueryAsync<AccountTransactionLogModel>("get_account_statement", p, null, commandType: CommandType.StoredProcedure);
    }

    private static short GetOutCode(DynamicParameters procedureParameters) => procedureParameters.Get<short>("out_code");
    private static int? GetOutCurrentBalance(DynamicParameters procedureParameters) => procedureParameters.Get<int?>("out_current_balance");
    private static int? GetOutCurrentLimit(DynamicParameters procedureParameters) => procedureParameters.Get<int?>("out_current_limit");
    private static DateTime GetOutStatementTimeStamp(DynamicParameters procedureParameters) => procedureParameters.Get<DateTime>("out_statement_timestamp");
}
