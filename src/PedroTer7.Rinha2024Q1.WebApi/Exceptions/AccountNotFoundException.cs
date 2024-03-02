namespace PedroTer7.Rinha2024Q1.WebApi.Exceptions;

public class AccountNotFoundException : Exception
{
    public int AccountId { get; }

    public AccountNotFoundException(int accountId) : base($"Account with ID {accountId} was not found")
    {
        AccountId = accountId;
    }
}
