using AutoMapper;
using PedroTer7.Rinha2024Q1.WebApi.Dtos;

namespace PedroTer7.Rinha2024Q1.WebApi.Mappers;

public class InToDataDtoMappingProfile : Profile
{
    public InToDataDtoMappingProfile()
    {
        CreateMap<InTransaction, TransactionDto>()
            .ConstructUsing(i => new TransactionDto(i.Valor, i.Tipo, i.Descricao));
    }
}
