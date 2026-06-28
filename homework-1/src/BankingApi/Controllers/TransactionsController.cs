using BankingApi.Models;
using BankingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("transactions")]
public class TransactionsController(ITransactionService service) : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] CreateTransactionRequest request)
    {
        var (transaction, errors) = service.CreateTransaction(request);

        if (errors.Count > 0)
            return BadRequest(new { errors });

        return CreatedAtAction(nameof(GetById), new { id = transaction!.Id }, transaction);
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] TransactionFilter filter)
    {
        return Ok(service.GetAll(filter));
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var transaction = service.GetById(id);

        if (transaction is null)
            return NotFound(new { error = "Transaction not found." });

        return Ok(transaction);
    }
}
