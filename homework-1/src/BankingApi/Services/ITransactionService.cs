using BankingApi.Models;

namespace BankingApi.Services;

public interface ITransactionService
{
    (Transaction? Transaction, List<string> Errors) CreateTransaction(CreateTransactionRequest request);
    Transaction? GetById(Guid id);
    List<Transaction> GetAll(TransactionFilter filter);
    AccountBalance GetBalance(string accountId);
    AccountSummary GetSummary(string accountId);
}
