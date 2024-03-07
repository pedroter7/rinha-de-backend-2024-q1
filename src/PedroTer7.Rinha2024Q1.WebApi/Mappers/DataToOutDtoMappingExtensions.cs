using PedroTer7.Rinha2024Q1.WebApi.Dtos;

namespace PedroTer7.Rinha2024Q1.WebApi.Mappers;

public static class DataToOutDtoMappingExtensions
{
    public static OutTransactionResult ToOutTransactionResult(this TransactionResultDto d) => new(d.Balance, d.Limit);

    public static OutAccountStatement ToOutAccountStatement(this AccountStatementDto d)
     => new(new OutAccountStatementBalance(d.Balance, d.TimeStamp, d.Limit),
                    d.Transactions.Select(o => new OutTransactionLog(Math.Abs(o.Value), o.TransactionType, o.Description, o.Timestamp)) ?? []);
}
