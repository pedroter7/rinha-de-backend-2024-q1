﻿using PedroTer7.Rinha2024Q1.Database.Services;
using PedroTer7.Rinha2024Q1.WebApi.Dtos;
using PedroTer7.Rinha2024Q1.WebApi.Exceptions;
using PedroTer7.Rinha2024Q1.Database.Enums;
using AutoMapper;
using PedroTer7.Rinha2024Q1.Database.Dtos;

namespace PedroTer7.Rinha2024Q1.WebApi.Services;

public class DataService(IDataAccessService dataAccessService, IMapper mapper) : IDataService
{
    private readonly IDataAccessService _dataAccessService = dataAccessService;
    private readonly IMapper _mapper = mapper;

    public async Task<TransactionResultDto> RegisterTransactionForAccount(int accountId, TransactionDto transaction)
    {
        char type = transaction.TransactionType[0];
        int amount = GetSignedAmount(transaction.Value, type);
        var procedureResult = await _dataAccessService.CallTransactionProcedure(accountId, type, amount, transaction.Description);

        ThrowIfProcedureResultIsNotSuccess(procedureResult, accountId);

        return _mapper.Map<TransactionResultDto>(procedureResult);
    }

    private static int GetSignedAmount(int amount, char transactionType) => transactionType switch
    {
        'c' => Math.Abs(amount),
        _ => -Math.Abs(amount),
    };

    public async Task<AccountStatementDto> GetAccountStatement(int accountId)
    {
        var procedureResult = await _dataAccessService.CallGetAccountStatementProcedure(accountId);

        ThrowIfProcedureResultIsNotSuccess(procedureResult, accountId);

        return _mapper.Map<AccountStatementDto>(procedureResult);
    }

    private static void ThrowIfProcedureResultIsNotSuccess(DataBaseProcedureResultDto dataBaseProcedureResult, int accountId)
    {
        switch (dataBaseProcedureResult.ResultCode)
        {
            case DataBaseProcedureResultCodeEnum.SUCCESS:
                return;
            case DataBaseProcedureResultCodeEnum.INVALID_TRANSACTION:
                throw new InvalidTransactionException();
            case DataBaseProcedureResultCodeEnum.INVALID_ACCOUNT:
                throw new AccountNotFoundException(accountId);
            case DataBaseProcedureResultCodeEnum.INVALID_OPERATION_ARGUMENTS:
                throw new ArgumentException("Database procedure call returned INVALID_OPERATION_ARGUMENTS result code");
            default:
                throw new Exception("Database procedure call returned an unknown result code");
        }

    }
}
