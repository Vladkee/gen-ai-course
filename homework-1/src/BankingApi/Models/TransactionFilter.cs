namespace BankingApi.Models;

public class TransactionFilter
{
    public string? AccountId { get; set; }
    public TransactionType? Type { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
