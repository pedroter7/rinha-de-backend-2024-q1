namespace PedroTer7.Rinha2024Q1.Database.Models;

public class AccountTransactionLogModel : IEquatable<AccountTransactionLogModel>
{
    public int Id { get; set; }
    public int Account_Id { get; set; }
    public short Type { get; set; }
    public int Amount { get; set; }
    public string Description { get; set; } = null!;
    public DateTime Timestamp_utc { get; set; }

    public bool Equals(AccountTransactionLogModel? other)
    {
        if (other is null) return false;
        return other.Id == this.Id;
    }
}