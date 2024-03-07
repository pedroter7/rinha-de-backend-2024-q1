using PedroTer7.Rinha2024Q1.WebApi.Dtos;

namespace PedroTer7.Rinha2024Q1.WebApi.Mappers;

public static class InToDataDtoMappingExtensions
{
    public static TransactionDto ToTransactionDto(this InTransaction i) => new(i.Valor, i.Tipo, i.Descricao);
}
