using PedroTer7.Rinha2024Q1.Database.Dtos;

namespace PedroTer7.Rinha2024Q1.Database.Services;

public interface IDataAccessService
{
    /// <summary>
    /// Calls the necessary db procedure to perform a transaction
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="operation"></param>
    /// <param name="amount">Transaction value signed according to operation type</param>
    /// <param name="description"></param>
    /// <returns></returns>
    Task<TransactionProcedureResultDto> CallTransactionProcedure(int accountId, char operation, int amount, string description);

    /// <summary>
    /// Calls the necessary db procedure to build an account statement
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    Task<GetAccountStatementProcedureResultDto> CallGetAccountStatementProcedure(int accountId);
}
