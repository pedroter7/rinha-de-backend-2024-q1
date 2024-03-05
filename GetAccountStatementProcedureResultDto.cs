using PedroTer7.Rinha2024Q1.Database.Enums;
using PedroTer7.Rinha2024Q1.Database.Models;

namespace PedroTer7.Rinha2024Q1.Database.Dtos;

public record class GetAccountStatementProcedureResultDto(DataBaseProcedureResultCodeEnum OperationResult, 
    int AccountBalance, int AccountLimit, DateTime StatementUtcTimestamp, IEnumerable<AccountTransactionLogModel> TransactionHistory) 
        : DataBaseProcedureResultDto(OperationResult);
