namespace BankingApi.Models;

public class AccountBalance
{
    public string AccountId { get; set; } = string.Empty;
    public Dictionary<string, decimal> Balances { get; set; } = new();
}
