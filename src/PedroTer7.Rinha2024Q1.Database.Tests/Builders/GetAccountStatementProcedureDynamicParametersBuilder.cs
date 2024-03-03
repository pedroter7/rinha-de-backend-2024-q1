namespace PedroTer7.Rinha2024Q1.Database.Tests.Builders;

public class GetAccountStatementProcedureDynamicParametersBuilder
{
    private long inAccountId;

    public GetAccountStatementProcedureDynamicParametersBuilder WithInAccountId(long accountId)
    {
        inAccountId = accountId;
        return this;
    }

    public DynamicParameters Build()
    {
        var parameters = new DynamicParameters();
        parameters.Add("in_account_id", inAccountId);
        parameters.Add("out_code", -1, dbType: DbType.Int16, direction: ParameterDirection.Output);
        parameters.Add("out_current_balance", null, dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("out_current_limit", null, dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("out_statement_timestamp", null, dbType: DbType.DateTime, direction: ParameterDirection.Output);

        return parameters;
    }
}
