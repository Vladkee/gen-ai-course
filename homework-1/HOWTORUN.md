# ▶️ How to Run

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio Code](https://code.visualstudio.com/)
- **REST Client** extension for VS Code ([humao.rest-client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client))
  - Open VS Code → Extensions (`Ctrl+Shift+X`) → search **REST Client** → Install

---

## Steps

**1. Install the REST Client extension** in VS Code (see above).

**2. Open a terminal and navigate to the project folder:**

```powershell
cd "homework-1"
```

**3. Start the API:**

```powershell
.\demo\start.ps1
```

The API will be available at `http://localhost:5000`.

**4. Open `demo/sample-requests.http`** in VS Code and click **Send Request** above any request block to execute it.

---

## 🧪 Testing Guide

### Option A — REST Client (VS Code)

Open `demo/sample-requests.http`. Each request block has a `### Comment` header. Click **Send Request** above any block. Responses appear in a split panel.

Included requests:
- Create Deposit, Withdrawal, Transfer
- Validation error cases (bad account format, invalid currency)
- List all transactions
- Filter by account / type / date range
- Get transaction by ID
- Get account balance and summary

### Option B — PowerShell demo script

Seeds four transactions and runs balance/summary calls automatically:

```powershell
.\demo\sample-data.ps1
```

### Option C — curl

```bash
# Create a transfer
curl -X POST http://localhost:5000/transactions \
  -H "Content-Type: application/json" \
  -d '{
    "fromAccount": "ACC-12345",
    "toAccount":   "ACC-67890",
    "amount":      100.50,
    "currency":    "USD",
    "type":        "Transfer"
  }'

# List all transactions
curl http://localhost:5000/transactions

# Filter by account
curl "http://localhost:5000/transactions?accountId=ACC-12345"

# Get account balance
curl http://localhost:5000/accounts/ACC-12345/balance
```

---

## Manual start (without the script)

```powershell
cd src/BankingApi
dotnet run
```
