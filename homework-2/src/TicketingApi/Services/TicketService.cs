using System.Collections.Concurrent;
using TicketingApi.Models;

namespace TicketingApi.Services;

public class TicketService : ITicketService
{
    private readonly ConcurrentDictionary<int, Ticket> _tickets = new();
    private int _nextId = 1;

    public Task<int> GetNextIdAsync()
    {
        return Task.FromResult(Interlocked.Increment(ref _nextId) - 1);
    }

    public async Task<Ticket> CreateTicketAsync(CreateTicketRequest request)
    {
        var id = await GetNextIdAsync();
        var ticket = new Ticket
        {
            Id = id,
            Subject = request.Subject,
            Description = request.Description,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Priority = request.Priority ?? TicketPriority.Medium,
            Category = request.Category ?? TicketCategory.General,
            Status = TicketStatus.Open,
            CreatedAt = DateTime.UtcNow,
            Tags = request.Tags ?? new List<string>()
        };

        _tickets[id] = ticket;
        return ticket;
    }

    public Task<Ticket?> GetTicketByIdAsync(int id)
    {
        _tickets.TryGetValue(id, out var ticket);
        return Task.FromResult(ticket);
    }

    public Task<List<Ticket>> GetTicketsAsync(TicketFilter? filter = null)
    {
        var tickets = _tickets.Values.AsEnumerable();

        if (filter is not null)
        {
            if (filter.Status.HasValue)
                tickets = tickets.Where(t => t.Status == filter.Status.Value);

            if (filter.Priority.HasValue)
                tickets = tickets.Where(t => t.Priority == filter.Priority.Value);

            if (filter.Category.HasValue)
                tickets = tickets.Where(t => t.Category == filter.Category.Value);

            if (filter.FromDate.HasValue)
                tickets = tickets.Where(t => t.CreatedAt >= filter.FromDate.Value);

            if (filter.ToDate.HasValue)
                tickets = tickets.Where(t => t.CreatedAt <= filter.ToDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.CustomerEmail))
                tickets = tickets.Where(t => t.CustomerEmail.Equals(filter.CustomerEmail, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult(tickets.OrderByDescending(t => t.CreatedAt).ToList());
    }

    public async Task<Ticket?> UpdateTicketAsync(int id, UpdateTicketRequest request)
    {
        var ticket = await GetTicketByIdAsync(id);
        if (ticket is null)
            return null;

        if (request.Subject is not null)
            ticket.Subject = request.Subject;

        if (request.Description is not null)
            ticket.Description = request.Description;

        if (request.Status.HasValue)
        {
            ticket.Status = request.Status.Value;
            if (request.Status.Value == TicketStatus.Resolved || request.Status.Value == TicketStatus.Closed)
                ticket.ResolvedAt ??= DateTime.UtcNow;
        }

        if (request.Priority.HasValue)
            ticket.Priority = request.Priority.Value;

        if (request.Category.HasValue)
            ticket.Category = request.Category.Value;

        if (request.AssignedTo is not null)
            ticket.AssignedTo = request.AssignedTo;

        if (request.Tags is not null)
            ticket.Tags = request.Tags;

        return ticket;
    }

    public Task<bool> DeleteTicketAsync(int id)
    {
        return Task.FromResult(_tickets.TryRemove(id, out _));
    }
}
