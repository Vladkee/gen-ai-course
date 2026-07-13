using TicketingApi.Models;

namespace TicketingApi.Services;

public interface IClassificationService
{
    Task<ClassificationResult> ClassifyTicketAsync(string subject, string description);
}
