using System.Data;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using PedroTer7.Rinha2024Q1.Database.Dtos;
using PedroTer7.Rinha2024Q1.Database.Enums;
using PedroTer7.Rinha2024Q1.Database.Models;
using PedroTer7.Rinha2024Q1.Database.Services;

namespace PedroTer7.Rinha2024Q1.Database;

internal class DataAccessService([FromKeyedServices("read")] MySqlConnection readConnection,
    [FromKeyedServices("write")] MySqlConnection writeConnection) : IDataAccessService
{
    private readonly MySqlConnection _readConnection = readConnection;
    private readonly MySqlConnection _writeConnection = writeConnection;

    public async Task<GetAccountStatementProcedureResultDto> CallGetAccountStatementProcedure(int accountId)
    {
        var p = new DynamicParameters();
        p.Add("in_account_id", accountId);
        p.Add("out_code", -1, dbType: DbType.Int16, direction: ParameterDirection.Output);
        p.Add("out_current_balance", null, dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("out_current_limit", null, dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("out_statement_timestamp", null, dbType: DbType.DateTime, direction: ParameterDirection.Output);

        var queryResult = await _readConnection.QueryAsync<AccountTransactionLogModel>("get_account_statement", p, commandType: CommandType.StoredProcedure);
        var outCode = p.Get<short>("out_code");
        var balance = p.Get<int?>("out_current_balance") ?? 0;
        var limit = p.Get<int?>("out_current_limit") ?? 0;
        var timestamp = p.Get<DateTime?>("out_statement_timestamp") ?? DateTime.UtcNow;
        return new GetAccountStatementProcedureResultDto((DataBaseProcedureResultCodeEnum)outCode, balance, limit, timestamp, queryResult);
    }

    public async Task<TransactionProcedureResultDto> CallTransactionProcedure(int accountId, char operation, int amount, string description)
    {
        var p = new DynamicParameters();
        p.Add("in_amount", amount);
        p.Add("in_account_id", accountId);
        p.Add("in_type", (short)operation);
        p.Add("in_description", description);
        p.Add("out_code", -1, dbType: DbType.Int16, direction: ParameterDirection.Output);
        p.Add("out_balance", null, dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("out_limit", null, dbType: DbType.Int32, direction: ParameterDirection.Output);

        await _writeConnection.ExecuteAsync("transaction", p, commandType: CommandType.StoredProcedure);
        var outCode = p.Get<short>("out_code");
        var balance = p.Get<int?>("out_balance") ?? 0;
        var limit = p.Get<int?>("out_limit") ?? 0;
        return new TransactionProcedureResultDto((DataBaseProcedureResultCodeEnum)outCode, balance, limit);
    }
}
