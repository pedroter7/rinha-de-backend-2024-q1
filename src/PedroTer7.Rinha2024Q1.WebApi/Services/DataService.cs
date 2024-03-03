using PedroTer7.Rinha2024Q1.Database.Services;
using PedroTer7.Rinha2024Q1.WebApi.Dtos;
using PedroTer7.Rinha2024Q1.WebApi.Exceptions;
using PedroTer7.Rinha2024Q1.Database.Enums;
using AutoMapper;

namespace PedroTer7.Rinha2024Q1.WebApi.Services;

public class DataService(IDataAccessService dataAccessService, IMapper mapper) : IDataService
{
    private readonly IDataAccessService _dataAccessService = dataAccessService;
    private readonly IMapper _mapper = mapper;

    public async Task<TransactionResultDto> RegisterTransactionForAccount(int accountId, TransactionDto transaction)
    {
        char type = transaction.TransactionType[0];
        int amount = type == 'c' ? transaction.Value : (-1) * transaction.Value;
        var procedureResult = await _dataAccessService.CallTransactionProcedure(accountId, type, amount, transaction.Description);
        if (procedureResult.OperationResult is DataBaseProcedureResultCodeEnum.INVALID_ACCOUNT)
            throw new AccountNotFoundException(accountId);
        else if (procedureResult.OperationResult is DataBaseProcedureResultCodeEnum.INVALID_OPERATION_ARGUMENTS)
            throw new ArgumentException(BuildInvalidTransactionArgumentsExceptionMessage(accountId, type, amount, transaction.Description));
        else if (procedureResult.OperationResult is DataBaseProcedureResultCodeEnum.INVALID_TRANSACTION)
            throw new InvalidTransactionException();

        return _mapper.Map<TransactionResultDto>(procedureResult);
    }

    private static string BuildInvalidTransactionArgumentsExceptionMessage(int accountId, char type, int amount, string description)
        => $"Transaction arguments invalid. Arguments were:\n\taccountId: {accountId}\n\ttype: {type}\n\tamount: {amount}\n\tdescription: {description}";

    public async Task<AccountStatementDto> GetAccountStatement(int accountId)
    {
        var procedureResult = await _dataAccessService.CallGetAccountStatementProcedure(accountId);
        if (procedureResult.OperationResult is DataBaseProcedureResultCodeEnum.INVALID_ACCOUNT)
            throw new AccountNotFoundException(accountId);

        return _mapper.Map<AccountStatementDto>(procedureResult);
    }
}
