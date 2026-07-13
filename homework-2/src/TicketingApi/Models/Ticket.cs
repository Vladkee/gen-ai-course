namespace TicketingApi.Models;

public class Ticket
{
    public int Id { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; }
    public TicketPriority Priority { get; set; }
    public TicketCategory Category { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public string? AssignedTo { get; set; }
    public List<string> Tags { get; set; } = new();
    public double? ClassificationConfidence { get; set; }
    public bool AutoClassified { get; set; }
}

public record CreateTicketRequest(
    string Subject,
    string Description,
    string CustomerName,
    string CustomerEmail,
    TicketPriority? Priority = null,
    TicketCategory? Category = null,
    List<string>? Tags = null,
    bool AutoClassify = false
);

public record UpdateTicketRequest(
    string? Subject = null,
    string? Description = null,
    TicketStatus? Status = null,
    TicketPriority? Priority = null,
    TicketCategory? Category = null,
    string? AssignedTo = null,
    List<string>? Tags = null
);

public record TicketFilter(
    TicketStatus? Status = null,
    TicketPriority? Priority = null,
    TicketCategory? Category = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? CustomerEmail = null
);
