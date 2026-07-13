using Microsoft.AspNetCore.Mvc;
using TicketingApi.Models;
using TicketingApi.Services;

namespace TicketingApi.Controllers;

[ApiController]
[Route("api/tickets")]
public class ImportController : ControllerBase
{
    private readonly IImportService _importService;

    public ImportController(IImportService importService)
    {
        _importService = importService;
    }

    [HttpPost("import")]
    public async Task<ActionResult<ImportSummary>> ImportTickets(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "File is required" });

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        ImportSummary summary;
        using var stream = file.OpenReadStream();

        try
        {
            summary = extension switch
            {
                ".csv" => await _importService.ImportFromCsvAsync(stream),
                ".json" => await _importService.ImportFromJsonAsync(stream),
                ".xml" => await _importService.ImportFromXmlAsync(stream),
                _ => throw new NotSupportedException($"File format '{extension}' is not supported. Use .csv, .json, or .xml")
            };
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Import failed: {ex.Message}" });
        }

        return Ok(summary);
    }
}
