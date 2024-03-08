using PedroTer7.Rinha2024Q1.Database.Dtos;
using PedroTer7.Rinha2024Q1.WebApi.Dtos;

namespace PedroTer7.Rinha2024Q1.WebApi.Mappers;

public static class ProcedureResultToDataDtoExtensions
{
    public static TransactionResultDto ToTransactionResultDto(this TransactionProcedureResultDto d) => new(d.NewAccountBalance, d.AccountLimit);

    public static AccountStatementDto ToAccountStatementDto(this GetAccountStatementProcedureResultDto d)
        => new(d.AccountBalance, d.AccountLimit,
                d.StatementUtcTimestamp,
                d.TransactionHistory
                    .Select(th => new TransactionLogDto(th.Amount, ((char)th.Type).ToString(), th.Description, th.Timestamp_utc))
                    .ToList());
}
