using System.Text;

namespace PedroTer7.Rinha2024Q1.Database.Tests;

public static class SqlQueriesUtil
{

    public static string BuildInsertIntoAccount(int accountsN)
    {
        return BuildInsertIntoAccount(accountsN, i => i * 100);
    }

    public static string BuildInsertIntoAccount(int accountsN, Func<int, int> calculateLimitFromAccountId)
    {
        StringBuilder stringBuilder = new("INSERT INTO account (id, `limit`) VALUES ");
        foreach (int i in Enumerable.Range(1, accountsN))
            stringBuilder.Append($"({i}, {calculateLimitFromAccountId(i)}){(i == accountsN ? ";" : ", ")}");
        return stringBuilder.ToString();
    }

    public static string BuildUpdateAccountBalanceCache(int accountId, int newBalanceValue)
    {
        return $"UPDATE account_balance_cache SET balance = {newBalanceValue} WHERE account_id = {accountId};";
    }

    public static string BuildInsertIntoTransactionLogs(int accountId, int nLogs)
    {
        StringBuilder stringBuilder = new("INSERT INTO account_transaction_log (account_id, `type`, amount, `description`, timestamp_utc) VALUES ");
        foreach (var i in Enumerable.Range(1, nLogs))
        {
            stringBuilder.Append($"({accountId}, 99, 1, 'desc', '{DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss")}'){(i == nLogs ? ";" : ", ")}");
            Task.Delay(100).Wait();
        }
        return stringBuilder.ToString();
    }

    public static string BuildSelectTransactionLogsForAccount(int accountId)
    {
        return $"SELECT * FROM account_transaction_log WHERE account_id = {accountId};";
    }
}
