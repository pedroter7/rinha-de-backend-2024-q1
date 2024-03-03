using PedroTer7.Rinha2024Q1.Database.Enums;

namespace PedroTer7.Rinha2024Q1.Database.Dtos;

public record class TransactionProcedureResultDto(DataBaseProcedureResultCodeEnum OperationResult, int NewAccountBalance, int AccountLimit);
