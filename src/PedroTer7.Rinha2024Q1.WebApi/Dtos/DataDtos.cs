namespace PedroTer7.Rinha2024Q1.WebApi.Dtos;

public record class TransactionDto(int Value, string TransactionType, string Description);

public record class TransactionResultDto(int Balance, int Limit);

public record class AccountStatementDto(int Balance, int Limit, DateTime TimeStamp, ICollection<TransactionLogDto> Transactions);

public record class TransactionLogDto(int Value, string TransactionType, string Description, DateTime Timestamp);