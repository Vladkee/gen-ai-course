using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using TicketingApi.Models;
using TicketingApi.Validation;

namespace TicketingApi.Services;

public class ImportService : IImportService
{
    private readonly ITicketService _ticketService;

    public ImportService(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    public async Task<ImportSummary> ImportFromCsvAsync(Stream csvStream)
    {
        var errors = new List<ImportError>();
        var successCount = 0;
        var totalRows = 0;

        try
        {
            using var reader = new StreamReader(csvStream);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using var csv = new CsvReader(reader, config);
            
            await csv.ReadAsync();
            csv.ReadHeader();

            var rowNumber = 1; // Header is row 0
            while (await csv.ReadAsync())
            {
                rowNumber++;
                totalRows++;

                try
                {
                    var subject = csv.GetField<string>("subject")?.Trim() ?? string.Empty;
                    var description = csv.GetField<string>("description")?.Trim() ?? string.Empty;
                    var customerName = csv.GetField<string>("customer_name")?.Trim() ?? string.Empty;
                    var customerEmail = csv.GetField<string>("customer_email")?.Trim() ?? string.Empty;
                    var priorityStr = csv.GetField<string>("priority")?.Trim();
                    var categoryStr = csv.GetField<string>("category")?.Trim();

                    var validationErrors = TicketValidator.ValidateCreateRequest(subject, description, customerName, customerEmail);
                    if (validationErrors.Any())
                    {
                        foreach (var error in validationErrors)
                        {
                            errors.Add(new ImportError(rowNumber, error.MemberNames.FirstOrDefault() ?? "unknown", error.ErrorMessage ?? "Validation failed"));
                        }
                        continue;
                    }

                    TicketPriority? priority = null;
                    if (!string.IsNullOrWhiteSpace(priorityStr) && Enum.TryParse<TicketPriority>(priorityStr, true, out var parsedPriority))
                        priority = parsedPriority;

                    TicketCategory? category = null;
                    if (!string.IsNullOrWhiteSpace(categoryStr) && Enum.TryParse<TicketCategory>(categoryStr, true, out var parsedCategory))
                        category = parsedCategory;

                    var request = new CreateTicketRequest(subject, description, customerName, customerEmail, priority, category);
                    await _ticketService.CreateTicketAsync(request);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new ImportError(rowNumber, "row", ex.Message));
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add(new ImportError(0, "file", $"Failed to parse CSV: {ex.Message}"));
        }

        return new ImportSummary(totalRows, successCount, errors.Count, errors);
    }

    public async Task<ImportSummary> ImportFromJsonAsync(Stream jsonStream)
    {
        var errors = new List<ImportError>();
        var successCount = 0;
        var totalRows = 0;

        try
        {
            var jsonDoc = await JsonDocument.ParseAsync(jsonStream);
            
            if (jsonDoc.RootElement.ValueKind != JsonValueKind.Array)
            {
                errors.Add(new ImportError(0, "file", "JSON root must be an array"));
                return new ImportSummary(0, 0, 1, errors);
            }

            var tickets = jsonDoc.RootElement.EnumerateArray();
            var rowNumber = 0;

            foreach (var ticketElement in tickets)
            {
                rowNumber++;
                totalRows++;

                try
                {
                    var subject = ticketElement.TryGetProperty("subject", out var subjectEl) ? subjectEl.GetString()?.Trim() ?? string.Empty : string.Empty;
                    var description = ticketElement.TryGetProperty("description", out var descEl) ? descEl.GetString()?.Trim() ?? string.Empty : string.Empty;
                    var customerName = ticketElement.TryGetProperty("customer_name", out var nameEl) ? nameEl.GetString()?.Trim() ?? string.Empty : string.Empty;
                    var customerEmail = ticketElement.TryGetProperty("customer_email", out var emailEl) ? emailEl.GetString()?.Trim() ?? string.Empty : string.Empty;

                    var validationErrors = TicketValidator.ValidateCreateRequest(subject, description, customerName, customerEmail);
                    if (validationErrors.Any())
                    {
                        foreach (var error in validationErrors)
                        {
                            errors.Add(new ImportError(rowNumber, error.MemberNames.FirstOrDefault() ?? "unknown", error.ErrorMessage ?? "Validation failed"));
                        }
                        continue;
                    }

                    TicketPriority? priority = null;
                    if (ticketElement.TryGetProperty("priority", out var priorityEl) && priorityEl.ValueKind == JsonValueKind.String)
                    {
                        var priorityStr = priorityEl.GetString();
                        if (!string.IsNullOrWhiteSpace(priorityStr) && Enum.TryParse<TicketPriority>(priorityStr, true, out var parsedPriority))
                            priority = parsedPriority;
                    }

                    TicketCategory? category = null;
                    if (ticketElement.TryGetProperty("category", out var categoryEl) && categoryEl.ValueKind == JsonValueKind.String)
                    {
                        var categoryStr = categoryEl.GetString();
                        if (!string.IsNullOrWhiteSpace(categoryStr) && Enum.TryParse<TicketCategory>(categoryStr, true, out var parsedCategory))
                            category = parsedCategory;
                    }

                    var request = new CreateTicketRequest(subject, description, customerName, customerEmail, priority, category);
                    await _ticketService.CreateTicketAsync(request);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new ImportError(rowNumber, "row", ex.Message));
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add(new ImportError(0, "file", $"Failed to parse JSON: {ex.Message}"));
        }

        return new ImportSummary(totalRows, successCount, errors.Count, errors);
    }

    public async Task<ImportSummary> ImportFromXmlAsync(Stream xmlStream)
    {
        var errors = new List<ImportError>();
        var successCount = 0;
        var totalRows = 0;

        try
        {
            var doc = await XDocument.LoadAsync(xmlStream, LoadOptions.None, CancellationToken.None);
            var ticketElements = doc.Descendants("ticket");

            var rowNumber = 0;
            foreach (var ticketElement in ticketElements)
            {
                rowNumber++;
                totalRows++;

                try
                {
                    var subject = ticketElement.Element("subject")?.Value.Trim() ?? string.Empty;
                    var description = ticketElement.Element("description")?.Value.Trim() ?? string.Empty;
                    var customerName = ticketElement.Element("customer_name")?.Value.Trim() ?? string.Empty;
                    var customerEmail = ticketElement.Element("customer_email")?.Value.Trim() ?? string.Empty;

                    var validationErrors = TicketValidator.ValidateCreateRequest(subject, description, customerName, customerEmail);
                    if (validationErrors.Any())
                    {
                        foreach (var error in validationErrors)
                        {
                            errors.Add(new ImportError(rowNumber, error.MemberNames.FirstOrDefault() ?? "unknown", error.ErrorMessage ?? "Validation failed"));
                        }
                        continue;
                    }

                    TicketPriority? priority = null;
                    var priorityStr = ticketElement.Element("priority")?.Value.Trim();
                    if (!string.IsNullOrWhiteSpace(priorityStr) && Enum.TryParse<TicketPriority>(priorityStr, true, out var parsedPriority))
                        priority = parsedPriority;

                    TicketCategory? category = null;
                    var categoryStr = ticketElement.Element("category")?.Value.Trim();
                    if (!string.IsNullOrWhiteSpace(categoryStr) && Enum.TryParse<TicketCategory>(categoryStr, true, out var parsedCategory))
                        category = parsedCategory;

                    var request = new CreateTicketRequest(subject, description, customerName, customerEmail, priority, category);
                    await _ticketService.CreateTicketAsync(request);
                    successCount++;
                }
                catch (Exception ex)
                {
                    errors.Add(new ImportError(rowNumber, "row", ex.Message));
                }
            }
        }
        catch (Exception ex)
        {
            errors.Add(new ImportError(0, "file", $"Failed to parse XML: {ex.Message}"));
        }

        return new ImportSummary(totalRows, successCount, errors.Count, errors);
    }
}
