using TicketingApi.Models;

namespace TicketingApi.Services;

public interface IImportService
{
    Task<ImportSummary> ImportFromCsvAsync(Stream csvStream);
    Task<ImportSummary> ImportFromJsonAsync(Stream jsonStream);
    Task<ImportSummary> ImportFromXmlAsync(Stream xmlStream);
}
