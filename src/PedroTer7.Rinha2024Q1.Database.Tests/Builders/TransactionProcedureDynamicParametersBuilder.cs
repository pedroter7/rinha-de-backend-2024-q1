namespace PedroTer7.Rinha2024Q1.Database.Tests.Builders;

public class TransactionProcedureDynamicParametersBuilder
{
    private int? inAmount;
    private int? inAccountId;
    private short? inType;
    private string inDescription = "a";

    public TransactionProcedureDynamicParametersBuilder WithInAmount(int? amount)
    {
        inAmount = amount;
        return this;
    }

    public TransactionProcedureDynamicParametersBuilder WithInAccountId(int? accountId)
    {
        inAccountId = accountId;
        return this;
    }

    public TransactionProcedureDynamicParametersBuilder WithInType(short? type)
    {
        inType = type;
        return this;
    }

    public TransactionProcedureDynamicParametersBuilder WithInDescription(string description)
    {
        inDescription = description;
        return this;
    }

    public DynamicParameters Build()
    {
        var parameters = new DynamicParameters();
        parameters.Add("in_amount", inAmount);
        parameters.Add("in_account_id", inAccountId);
        parameters.Add("in_type", inType);
        parameters.Add("in_description", inDescription);
        parameters.Add("out_code", -1, dbType: DbType.Int16, direction: ParameterDirection.Output);
        parameters.Add("out_balance", null, dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("out_limit", null, dbType: DbType.Int32, direction: ParameterDirection.Output);

        return parameters;
    }
}
