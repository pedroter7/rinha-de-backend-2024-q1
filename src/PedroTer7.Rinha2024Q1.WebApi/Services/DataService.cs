using PedroTer7.Rinha2024Q1.WebApi.Dtos;

namespace PedroTer7.Rinha2024Q1.WebApi.Services;

public class DataService : IDataService
{
    public Task<TransactionResultDto> RegisterTransactionForAccount(int accountId, TransactionDto transaction)
    {
        throw new NotImplementedException();
    }

    public Task<AccountStatementDto> GetAccountStatement(int accountId)
    {
        throw new NotImplementedException();
    }
}
