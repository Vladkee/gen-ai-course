namespace TicketingApi.Models;

public enum TicketStatus
{
    Open,
    InProgress,
    Resolved,
    Closed
}

public enum TicketPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum TicketCategory
{
    Technical,
    Billing,
    Account,
    General
}
