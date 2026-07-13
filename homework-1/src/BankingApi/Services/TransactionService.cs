using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using BankingApi.Models;
using BankingApi.Validation;

namespace BankingApi.Services;

public class TransactionService : ITransactionService
{
    private static readonly Regex AccountPattern = new(@"^ACC-\d{5}$", RegexOptions.Compiled);
    private readonly ConcurrentDictionary<Guid, Transaction> _store = new();

    public (Transaction? Transaction, List<string> Errors) CreateTransaction(CreateTransactionRequest request)
    {
        var errors = Validate(request);
        if (errors.Count > 0)
            return (null, errors);

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            FromAccount = request.FromAccount?.ToUpperInvariant(),
            ToAccount = request.ToAccount?.ToUpperInvariant(),
            Amount = Math.Round(request.Amount, 2),
            Currency = request.Currency.ToUpperInvariant(),
            Type = request.Type,
            Timestamp = DateTime.UtcNow,
            Status = TransactionStatus.Completed
        };

        _store[transaction.Id] = transaction;
        return (transaction, errors);
    }

    public Transaction? GetById(Guid id) =>
        _store.TryGetValue(id, out var t) ? t : null;

    public List<Transaction> GetAll(TransactionFilter filter)
    {
        IEnumerable<Transaction> query = _store.Values;

        if (!string.IsNullOrEmpty(filter.AccountId))
        {
            var accountId = filter.AccountId.ToUpperInvariant();
            query = query.Where(t => t.FromAccount == accountId || t.ToAccount == accountId);
        }

        if (filter.Type.HasValue)
            query = query.Where(t => t.Type == filter.Type.Value);

        if (filter.FromDate.HasValue)
            query = query.Where(t => t.Timestamp >= filter.FromDate.Value);

        if (filter.ToDate.HasValue)
            query = query.Where(t => t.Timestamp <= filter.ToDate.Value);

        return query.OrderByDescending(t => t.Timestamp).ToList();
    }

    public AccountBalance GetBalance(string accountId)
    {
        var id = accountId.ToUpperInvariant();
        var balances = new Dictionary<string, decimal>();

        foreach (var t in _store.Values.Where(t => t.FromAccount == id || t.ToAccount == id))
        {
            balances.TryAdd(t.Currency, 0m);

            if (t.ToAccount == id)
                balances[t.Currency] += t.Amount;

            if (t.FromAccount == id)
                balances[t.Currency] -= t.Amount;
        }

        return new AccountBalance { AccountId = id, Balances = balances };
    }

    public AccountSummary GetSummary(string accountId)
    {
        var id = accountId.ToUpperInvariant();
        var related = _store.Values
            .Where(t => t.FromAccount == id || t.ToAccount == id)
            .ToList();

        var depositsTotal = related
            .Where(t => t.Type == TransactionType.Deposit && t.ToAccount == id)
            .Sum(t => t.Amount);

        var withdrawalsTotal = related
            .Where(t => t.Type == TransactionType.Withdrawal && t.FromAccount == id)
            .Sum(t => t.Amount);

        return new AccountSummary
        {
            AccountId = id,
            DepositsTotal = depositsTotal,
            WithdrawalsTotal = withdrawalsTotal,
            TransactionCount = related.Count,
            MostRecentDate = related.Count > 0 ? related.Max(t => t.Timestamp) : null
        };
    }

    private static List<string> Validate(CreateTransactionRequest request)
    {
        var errors = new List<string>();

        if (request.Amount <= 0)
            errors.Add("Amount must be positive.");
        else if (request.Amount != Math.Round(request.Amount, 2))
            errors.Add("Amount must have at most 2 decimal places.");

        if (!Iso4217Currencies.IsValid(request.Currency))
            errors.Add($"'{request.Currency}' is not a valid ISO 4217 currency code.");

        if (!string.IsNullOrEmpty(request.FromAccount) && !AccountPattern.IsMatch(request.FromAccount))
            errors.Add($"FromAccount '{request.FromAccount}' must match format ACC-XXXXX (5 digits).");

        if (!string.IsNullOrEmpty(request.ToAccount) && !AccountPattern.IsMatch(request.ToAccount))
            errors.Add($"ToAccount '{request.ToAccount}' must match format ACC-XXXXX (5 digits).");

        switch (request.Type)
        {
            case TransactionType.Deposit:
                if (string.IsNullOrEmpty(request.ToAccount))
                    errors.Add("ToAccount is required for Deposit transactions.");
                break;
            case TransactionType.Withdrawal:
                if (string.IsNullOrEmpty(request.FromAccount))
                    errors.Add("FromAccount is required for Withdrawal transactions.");
                break;
            case TransactionType.Transfer:
                if (string.IsNullOrEmpty(request.FromAccount))
                    errors.Add("FromAccount is required for Transfer transactions.");
                if (string.IsNullOrEmpty(request.ToAccount))
                    errors.Add("ToAccount is required for Transfer transactions.");
                if (!string.IsNullOrEmpty(request.FromAccount) &&
                    request.FromAccount.Equals(request.ToAccount, StringComparison.OrdinalIgnoreCase))
                    errors.Add("FromAccount and ToAccount must be different for Transfer transactions.");
                break;
        }

        return errors;
    }
}
