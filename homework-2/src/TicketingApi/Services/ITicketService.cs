using TicketingApi.Models;

namespace TicketingApi.Services;

public interface ITicketService
{
    Task<Ticket> CreateTicketAsync(CreateTicketRequest request);
    Task<Ticket?> GetTicketByIdAsync(int id);
    Task<List<Ticket>> GetTicketsAsync(TicketFilter? filter = null);
    Task<Ticket?> UpdateTicketAsync(int id, UpdateTicketRequest request);
    Task<bool> DeleteTicketAsync(int id);
    Task<int> GetNextIdAsync();
}
