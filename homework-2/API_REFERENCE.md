# API Reference — Ticketing API

Base URL: `http://localhost:5002/api`

## Endpoints

### Tickets

#### POST /tickets
Create a new support ticket.

**Request Body:**
```json
{
  "subject": "string (required, max 200 chars)",
  "description": "string (required, max 2000 chars)",
  "customerName": "string (required, max 100 chars)",
  "customerEmail": "string (required, valid email)",
  "priority": "Low|Medium|High|Critical (optional, default: Medium)",
  "category": "Technical|Billing|Account|General (optional, default: General)",
  "tags": ["string"] (optional),
  "autoClassify": boolean (optional, default: false)
}
```

**Success Response: `201 Created`**
```json
{
  "id": 1,
  "subject": "Login issue",
  "description": "Cannot log in to my account",
  "status": "Open",
  "priority": "Medium",
  "category": "General",
  "customerName": "John Doe",
  "customerEmail": "john@example.com",
  "createdAt": "2026-07-05T18:00:00Z",
  "resolvedAt": null,
  "assignedTo": null,
  "tags": [],
  "classificationConfidence": null,
  "autoClassified": false
}
```

**Error Response: `400 Bad Request`**
```json
{
  "message": "Validation failed",
  "errors": [
    {
      "field": "customerEmail",
      "message": "Customer email is not valid"
    }
  ]
}
```

---

#### GET /tickets
Retrieve all tickets with optional filtering.

**Query Parameters:**
- `status`: `Open|InProgress|Resolved|Closed`
- `priority`: `Low|Medium|High|Critical`
- `category`: `Technical|Billing|Account|General`
- `fromDate`: ISO 8601 datetime (e.g., `2026-07-01T00:00:00Z`)
- `toDate`: ISO 8601 datetime
- `customerEmail`: string

**Example:**
```
GET /tickets?status=Open&priority=High
```

**Success Response: `200 OK`**
```json
[
  {
    "id": 1,
    "subject": "Login issue",
    "description": "Cannot log in",
    "status": "Open",
    "priority": "High",
    "category": "Technical",
    "customerName": "John Doe",
    "customerEmail": "john@example.com",
    "createdAt": "2026-07-05T18:00:00Z",
    "resolvedAt": null,
    "assignedTo": null,
    "tags": [],
    "classificationConfidence": null,
    "autoClassified": false
  }
]
```

---

#### GET /tickets/{id}
Retrieve a single ticket by ID.

**Path Parameters:**
- `id`: integer

**Success Response: `200 OK`**
```json
{
  "id": 1,
  "subject": "Login issue",
  "description": "Cannot log in to my account",
  "status": "Open",
  "priority": "Medium",
  "category": "Technical",
  "customerName": "John Doe",
  "customerEmail": "john@example.com",
  "createdAt": "2026-07-05T18:00:00Z",
  "resolvedAt": null,
  "assignedTo": null,
  "tags": [],
  "classificationConfidence": 0.8,
  "autoClassified": true
}
```

**Error Response: `404 Not Found`**
```json
{
  "message": "Ticket 999 not found"
}
```

---

#### PUT /tickets/{id}
Update an existing ticket.

**Path Parameters:**
- `id`: integer

**Request Body (all fields optional):**
```json
{
  "subject": "string (max 200 chars)",
  "description": "string (max 2000 chars)",
  "status": "Open|InProgress|Resolved|Closed",
  "priority": "Low|Medium|High|Critical",
  "category": "Technical|Billing|Account|General",
  "assignedTo": "string (max 100 chars)",
  "tags": ["string"]
}
```

**Success Response: `200 OK`**
```json
{
  "id": 1,
  "subject": "Login issue - RESOLVED",
  "description": "Cannot log in to my account",
  "status": "Resolved",
  "priority": "Medium",
  "category": "Technical",
  "customerName": "John Doe",
  "customerEmail": "john@example.com",
  "createdAt": "2026-07-05T18:00:00Z",
  "resolvedAt": "2026-07-05T19:30:00Z",
  "assignedTo": "Agent Smith",
  "tags": ["resolved", "password-reset"],
  "classificationConfidence": null,
  "autoClassified": false
}
```

**Error Responses:**
- `404 Not Found`: Ticket not found
- `400 Bad Request`: Validation failed

---

#### DELETE /tickets/{id}
Delete a ticket.

**Path Parameters:**
- `id`: integer

**Success Response: `204 No Content`**

**Error Response: `404 Not Found`**
```json
{
  "message": "Ticket 999 not found"
}
```

---

#### POST /tickets/{id}/auto-classify
Auto-classify an existing ticket using keyword analysis.

**Path Parameters:**
- `id`: integer

**Success Response: `200 OK`**
```json
{
  "id": 1,
  "subject": "Urgent payment error",
  "description": "Critical billing issue need refund",
  "status": "Open",
  "priority": "Critical",
  "category": "Billing",
  "customerName": "John Doe",
  "customerEmail": "john@example.com",
  "createdAt": "2026-07-05T18:00:00Z",
  "resolvedAt": null,
  "assignedTo": null,
  "tags": [],
  "classificationConfidence": 0.8,
  "autoClassified": true
}
```

**Error Response: `404 Not Found`**

---

### Import

#### POST /tickets/import
Bulk import tickets from CSV, JSON, or XML file.

**Request:**
- Content-Type: `multipart/form-data`
- Form field: `file` (CSV/JSON/XML file)

**Supported Formats:**

**CSV:**
```csv
subject,description,customer_name,customer_email,priority,category
Login issue,Cannot login,John Doe,john@example.com,High,Technical
```

**JSON:**
```json
[
  {
    "subject": "Login issue",
    "description": "Cannot login",
    "customer_name": "John Doe",
    "customer_email": "john@example.com",
    "priority": "High",
    "category": "Technical"
  }
]
```

**XML:**
```xml
<tickets>
  <ticket>
    <subject>Login issue</subject>
    <description>Cannot login</description>
    <customer_name>John Doe</customer_name>
    <customer_email>john@example.com</customer_email>
    <priority>High</priority>
    <category>Technical</category>
  </ticket>
</tickets>
```

**Success Response: `200 OK`**
```json
{
  "totalRows": 10,
  "successCount": 8,
  "errorCount": 2,
  "errors": [
    {
      "rowNumber": 3,
      "field": "customerEmail",
      "errorMessage": "Customer email is not valid"
    },
    {
      "rowNumber": 7,
      "field": "subject",
      "errorMessage": "Subject is required"
    }
  ]
}
```

**Error Responses:**
- `400 Bad Request`: File missing, invalid format, or parsing error

---

## Data Models

### Ticket

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `id` | int | Auto | Auto-incremented |
| `subject` | string | Yes | Max 200 characters |
| `description` | string | Yes | Max 2000 characters |
| `status` | enum | Auto | Open, InProgress, Resolved, Closed |
| `priority` | enum | No | Low, Medium (default), High, Critical |
| `category` | enum | No | Technical, Billing, Account, General (default) |
| `customerName` | string | Yes | Max 100 characters |
| `customerEmail` | string | Yes | Valid email format |
| `createdAt` | datetime | Auto | UTC timestamp |
| `resolvedAt` | datetime | Auto | Set when status = Resolved/Closed |
| `assignedTo` | string | No | Max 100 characters |
| `tags` | string[] | No | Array of tags |
| `classificationConfidence` | double | Auto | 0.0-1.0, set by auto-classify |
| `autoClassified` | boolean | Auto | true if auto-classified |

### TicketStatus Enum

- `Open` — Newly created ticket
- `InProgress` — Being worked on
- `Resolved` — Solution provided
- `Closed` — Ticket closed

### TicketPriority Enum

- `Low` — Non-urgent
- `Medium` — Normal priority (default)
- `High` — Important
- `Critical` — Urgent, system down

### TicketCategory Enum

- `Technical` — Technical issues
- `Billing` — Billing, payments, invoices
- `Account` — Account management
- `General` — General inquiries (default)

---

## Error Handling

All error responses follow a consistent format:

```json
{
  "message": "Human-readable error message"
}
```

For validation errors:

```json
{
  "message": "Validation failed",
  "errors": [
    {
      "field": "fieldName",
      "message": "Specific error message"
    }
  ]
}
```

---

## Auto-Classification

The auto-classification engine uses keyword matching to suggest category and priority.

### Keywords Used

**Category:**
- **Technical**: login, password, error, bug, crash, slow, performance
- **Billing**: payment, charge, invoice, refund, subscription, bill
- **Account**: account, profile, settings, delete, reset
- **General**: default when no match

**Priority:**
- **Critical**: urgent, critical, asap, immediately, down, outage
- **High**: important, high, soon, blocking
- **Low**: question, help, how
- **Medium**: default

### Confidence Score

Score ranges from 0.0 to 1.0, calculated as:
```
confidence = min(1.0, keyword_matches * 0.2)
```

Examples:
- 0 matches = 0.0 (no confidence)
- 3 matches = 0.6 (60% confidence)
- 5+ matches = 1.0 (100% confidence)

---

## Rate Limits

Currently: **No rate limits** (for homework demonstration).

Production recommendation: 100 requests/minute per IP.

---

## Examples

### Create Ticket with Auto-Classify

```bash
curl -X POST http://localhost:5002/api/tickets \
  -H "Content-Type: application/json" \
  -d '{
    "subject": "URGENT: System down",
    "description": "Critical error all users affected",
    "customerName": "John Doe",
    "customerEmail": "john@example.com",
    "autoClassify": true
  }'
```

### Get High Priority Tickets

```bash
curl "http://localhost:5002/api/tickets?priority=High"
```

### Update Ticket Status

```bash
curl -X PUT http://localhost:5002/api/tickets/1 \
  -H "Content-Type: application/json" \
  -d '{
    "status": "Resolved",
    "assignedTo": "Agent Smith"
  }'
```

### Import CSV File

```bash
curl -X POST http://localhost:5002/api/tickets/import \
  -F "file=@sample_tickets.csv"
```
