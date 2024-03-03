using AutoMapper;
using PedroTer7.Rinha2024Q1.Database.Dtos;
using PedroTer7.Rinha2024Q1.WebApi.Dtos;

namespace PedroTer7.Rinha2024Q1.WebApi.Mappers;

public class ProcedureResultToDataDtoMappingProfile : Profile
{
    public ProcedureResultToDataDtoMappingProfile()
    {
        CreateMap<GetAccountStatementProcedureResultDto, AccountStatementDto>()
            .ConvertUsing(x => new AccountStatementDto(x.AccountBalance, x.AccountLimit,
                x.StatementUtcTimestamp,
                x.TransactionHistory
                    .Select(th => new TransactionLogDto(th.Amount, ((char)th.Type).ToString(), th.Description, th.Timestamp_utc))
                    .ToList()));

        CreateMap<TransactionProcedureResultDto, TransactionResultDto>()
            .ConvertUsing(x => new TransactionResultDto(x.NewAccountBalance, x.AccountLimit));
    }
}
