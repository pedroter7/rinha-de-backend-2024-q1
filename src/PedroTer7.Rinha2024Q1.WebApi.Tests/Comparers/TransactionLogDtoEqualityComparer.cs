using System.Diagnostics.CodeAnalysis;
using PedroTer7.Rinha2024Q1.WebApi.Dtos;

namespace PedroTer7.Rinha2024Q1.WebApi.Tests;

public class TransactionLogDtoEqualityComparer : IEqualityComparer<TransactionLogDto>
{
    public bool Equals(TransactionLogDto? x, TransactionLogDto? y)
    {
        if (x is null && y is null) return true;
        else if (x is null || y is null) return false;
        return x.Description.Equals(y.Description)
            && x.Value == y.Value && x.Timestamp == y.Timestamp
            && x.TransactionType == y.TransactionType;
    }

    public int GetHashCode([DisallowNull] TransactionLogDto obj)
    {
        throw new NotImplementedException();
    }
}
