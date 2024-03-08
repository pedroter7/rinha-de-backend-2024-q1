using AutoMapper;
using PedroTer7.Rinha2024Q1.WebApi.Dtos;

namespace PedroTer7.Rinha2024Q1.WebApi.Mappers;

public class DataToOutDtoMappingProfile : Profile
{
    public DataToOutDtoMappingProfile()
    {
        CreateMap<TransactionResultDto, OutTransactionResult>()
            .ConstructUsing(d => new OutTransactionResult(d.Balance, d.Limit));

        CreateMap<AccountStatementDto, OutAccountStatement>()
            .ConstructUsing(d =>
                new OutAccountStatement(new OutAccountStatementBalance(d.Balance, d.TimeStamp, d.Limit),
                    d.Transactions.Select(o => new OutTransactionLog(Math.Abs(o.Value), o.TransactionType, o.Description, o.Timestamp)) ?? new List<OutTransactionLog>()));
    }
}
