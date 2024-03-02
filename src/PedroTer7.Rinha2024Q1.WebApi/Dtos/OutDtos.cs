namespace PedroTer7.Rinha2024Q1.WebApi.Dtos;

public record class OutTransactionResult(int Saldo, int Limite);

public record class OutAccountStatement(OutAccountStatementBalance Saldo, IEnumerable<OutTransactionLog> Ultimas_Transacoes);

public record class OutAccountStatementBalance(int Total, DateTime Data_extrato, int Limite);

public record class OutTransactionLog(int Valor, string Tipo, string Descricao, DateTime Realizada_em);