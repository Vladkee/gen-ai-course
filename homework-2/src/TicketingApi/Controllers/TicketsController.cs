using Microsoft.AspNetCore.Mvc;
using TicketingApi.Models;
using TicketingApi.Services;
using TicketingApi.Validation;

namespace TicketingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IClassificationService _classificationService;

    public TicketsController(ITicketService ticketService, IClassificationService classificationService)
    {
        _ticketService = ticketService;
        _classificationService = classificationService;
    }

    [HttpPost]
    public async Task<ActionResult<Ticket>> CreateTicket([FromBody] CreateTicketRequest request)
    {
        var validationErrors = TicketValidator.ValidateCreateRequest(
            request.Subject,
            request.Description,
            request.CustomerName,
            request.CustomerEmail
        );

        if (validationErrors.Any())
        {
            return BadRequest(new
            {
                message = "Validation failed",
                errors = validationErrors.Select(e => new { field = e.MemberNames.FirstOrDefault(), message = e.ErrorMessage })
            });
        }

        var ticket = await _ticketService.CreateTicketAsync(request);

        if (request.AutoClassify)
        {
            var classification = await _classificationService.ClassifyTicketAsync(ticket.Subject, ticket.Description);
            ticket.Category = classification.SuggestedCategory;
            ticket.Priority = classification.SuggestedPriority;
            ticket.ClassificationConfidence = classification.Confidence;
            ticket.AutoClassified = true;
        }

        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpGet]
    public async Task<ActionResult<List<Ticket>>> GetTickets(
        [FromQuery] TicketStatus? status = null,
        [FromQuery] TicketPriority? priority = null,
        [FromQuery] TicketCategory? category = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? customerEmail = null)
    {
        var filter = new TicketFilter(status, priority, category, fromDate, toDate, customerEmail);
        var tickets = await _ticketService.GetTicketsAsync(filter);
        return Ok(tickets);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicket(int id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket is null)
            return NotFound(new { message = $"Ticket {id} not found" });

        return Ok(ticket);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Ticket>> UpdateTicket(int id, [FromBody] UpdateTicketRequest request)
    {
        var validationErrors = TicketValidator.ValidateUpdateRequest(
            request.Subject,
            request.Description,
            request.AssignedTo
        );

        if (validationErrors.Any())
        {
            return BadRequest(new
            {
                message = "Validation failed",
                errors = validationErrors.Select(e => new { field = e.MemberNames.FirstOrDefault(), message = e.ErrorMessage })
            });
        }

        var ticket = await _ticketService.UpdateTicketAsync(id, request);
        if (ticket is null)
            return NotFound(new { message = $"Ticket {id} not found" });

        return Ok(ticket);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(int id)
    {
        var deleted = await _ticketService.DeleteTicketAsync(id);
        if (!deleted)
            return NotFound(new { message = $"Ticket {id} not found" });

        return NoContent();
    }

    [HttpPost("{id}/auto-classify")]
    public async Task<ActionResult<Ticket>> AutoClassifyTicket(int id)
    {
        var ticket = await _ticketService.GetTicketByIdAsync(id);
        if (ticket is null)
            return NotFound(new { message = $"Ticket {id} not found" });

        var classification = await _classificationService.ClassifyTicketAsync(ticket.Subject, ticket.Description);
        
        var updateRequest = new UpdateTicketRequest(
            Category: classification.SuggestedCategory,
            Priority: classification.SuggestedPriority
        );

        ticket = await _ticketService.UpdateTicketAsync(id, updateRequest);
        ticket!.ClassificationConfidence = classification.Confidence;
        ticket.AutoClassified = true;

        return Ok(ticket);
    }
}
