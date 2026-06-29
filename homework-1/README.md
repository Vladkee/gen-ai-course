# 🏦 Homework 1: Banking Transactions API

> **Student Name**: Vlad Radchenko
> **GitHub**: [@Vladkee](https://github.com/Vladkee)
> **Date Submitted**: 2026-06-28
> **AI Tools Used**: Claude CLI (Sonnet 4.6), Claude Desktop (Opus 4.8)

---

## 📋 Project Overview

A minimal REST API for managing banking transactions, built with **ASP.NET Core 8** and in-memory storage. No database is required — data lives in a `ConcurrentDictionary` for the lifetime of the process.

### Features

- Create and retrieve transactions (**Deposit**, **Withdrawal**, **Transfer**)
- Field-level validation: positive amounts, max 2 decimal places, `ACC-XXXXX` account format, ISO 4217 currency codes
- Filter transactions by account, type, and date range (filters can be combined)
- Account balance calculation per currency
- **Optional feature:** account transaction summary (deposits total, withdrawals total, count, most recent date)

---

## 🔗 Endpoints

| Method | Path | Description |
|--------|------|-------------|
| `POST` | `/transactions` | Create a transaction |
| `GET` | `/transactions` | List all transactions (with optional filters) |
| `GET` | `/transactions/{id}` | Get a transaction by ID |
| `GET` | `/accounts/{accountId}/balance` | Get account balance per currency |
| `GET` | `/accounts/{accountId}/summary` | Get account transaction summary *(optional feature)* |

---

## 📦 Transaction Model

```json
{
  "id": "18646db4-b615-4c11-aa42-d57df0b45eb0",
  "fromAccount": "ACC-12345",
  "toAccount": "ACC-67890",
  "amount": 100.50,
  "currency": "USD",
  "type": "Transfer",
  "timestamp": "2026-06-28T17:53:31Z",
  "status": "Completed"
}
```

| Field | Required | Notes |
|-------|----------|-------|
| `fromAccount` | Withdrawal, Transfer | Format: `ACC-XXXXX` (5 digits) |
| `toAccount` | Deposit, Transfer | Format: `ACC-XXXXX` (5 digits) |
| `amount` | Yes | Positive, max 2 decimal places |
| `currency` | Yes | ISO 4217 code — `USD`, `EUR`, `GBP`, etc. |
| `type` | Yes | `Deposit`, `Withdrawal`, or `Transfer` (case-insensitive) |

---

## 🔍 Filter Parameters (`GET /transactions`)

| Parameter | Description |
|-----------|-------------|
| `accountId` | Matches `fromAccount` or `toAccount` |
| `type` | `Deposit`, `Withdrawal`, or `Transfer` |
| `fromDate` | Start of date range, inclusive (ISO 8601 UTC) |
| `toDate` | End of date range, inclusive (ISO 8601 UTC) |

All parameters are optional and can be combined.

---

## ✅ Validation Rules

- **Amount**: must be positive; max 2 decimal places
- **Account number**: must match `ACC-XXXXX` (exactly 5 digits)
- **Currency**: must be a valid ISO 4217 code
- **Deposit**: `toAccount` is required
- **Withdrawal**: `fromAccount` is required
- **Transfer**: both accounts required and must be different

Error responses return HTTP **400** with field-level messages:
```json
{ "errors": ["'XYZ' is not a valid ISO 4217 currency code."] }
```

---

## 📁 Project Structure

```
homework-1/
├── src/
│   └── BankingApi/
│       ├── Controllers/
│       │   ├── TransactionsController.cs    # POST/GET /transactions
│       │   └── AccountsController.cs        # GET /accounts/{id}/balance|summary
│       ├── Models/
│       │   ├── Transaction.cs               # Stored transaction entity
│       │   ├── CreateTransactionRequest.cs  # POST request body
│       │   ├── TransactionFilter.cs         # Query-string filter binding
│       │   ├── AccountBalance.cs            # Balance response (per-currency)
│       │   ├── AccountSummary.cs            # Summary response (optional feature)
│       │   └── Enums.cs                     # TransactionType, TransactionStatus
│       ├── Services/
│       │   ├── ITransactionService.cs       # Service contract
│       │   └── TransactionService.cs        # Business logic, validation, in-memory store
│       ├── Validation/
│       │   └── Iso4217Currencies.cs         # ISO 4217 currency code lookup
│       ├── Program.cs                       # App bootstrap & DI wiring
│       └── BankingApi.csproj
├── demo/
│   ├── start.ps1                            # Launch the API
│   ├── sample-data.ps1                      # Seed sample data & demo calls
│   └── sample-requests.http                 # REST Client / Postman requests
├── docs/
│   ├── AI-NOTES.md                          # AI tool usage notes
│   └── screenshots/                         # Screenshots of API & AI interactions
├── README.md                                # This file
├── HOWTORUN.md                              # Setup & run instructions
└── TASKS.md                                 # Original assignment tasks
```

---

## 🤖 AI Tool Usage

| Phase | Tool | Model | Purpose |
|-------|------|-------|---------|
| Initial creation | Claude CLI | Sonnet 4.6 | Scaffolded the full project — models, controllers, validation, service layer |
| Review & polish | Claude Desktop | Opus 4.8 | Caught field-name mismatch vs. spec, aligned docs/config, full test suite |

See [`docs/AI-NOTES.md`](docs/AI-NOTES.md) for detailed notes.

> ⚠️ The Claude CLI chat session from Phase 1 was lost when the terminal window was closed — screenshots of that interaction are not available. Phase 2 (Claude Desktop) review produced the working, spec-compliant version.

---

<div align="center">

*This project was completed as part of the AI-Assisted Development course.*

</div>
