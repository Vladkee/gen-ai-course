namespace BankingApi.Models;

public class CreateTransactionRequest
{
    public string? FromAccount { get; set; }
    public string? ToAccount { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
}
