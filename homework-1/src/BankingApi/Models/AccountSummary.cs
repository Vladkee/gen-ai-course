namespace BankingApi.Models;

public class AccountSummary
{
    public string AccountId { get; set; } = string.Empty;
    public decimal DepositsTotal { get; set; }
    public decimal WithdrawalsTotal { get; set; }
    public int TransactionCount { get; set; }
    public DateTime? MostRecentDate { get; set; }
}
