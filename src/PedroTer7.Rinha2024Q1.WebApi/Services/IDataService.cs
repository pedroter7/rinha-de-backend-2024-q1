using PedroTer7.Rinha2024Q1.WebApi.Dtos;
using PedroTer7.Rinha2024Q1.WebApi.Exceptions;

namespace PedroTer7.Rinha2024Q1.WebApi.Services;

public interface IDataService
{
    /// <summary>
    /// Registers the given transaction for the given account
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="transaction"></param>
    /// <returns></returns>
    /// <exception cref="AccountNotFoundException"></exception>
    /// <exception cref="InvalidTransactionException"></exception>
    Task<TransactionResultDto> RegisterTransactionForAccount(int accountId, TransactionDto transaction);

    /// <summary>
    /// Gets data and builds account statement for the given account
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    /// <exception cref="AccountNotFoundException"></exception>
    Task<AccountStatementDto> GetAccountStatement(int accountId);
}
