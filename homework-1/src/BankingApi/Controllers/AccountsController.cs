using System.Text.RegularExpressions;
using BankingApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingApi.Controllers;

[ApiController]
[Route("accounts")]
public class AccountsController(ITransactionService service) : ControllerBase
{
    private static readonly Regex AccountPattern = new(@"^ACC-\d{5}$", RegexOptions.Compiled);

    [HttpGet("{accountId}/balance")]
    public IActionResult GetBalance(string accountId)
    {
        if (!AccountPattern.IsMatch(accountId))
            return BadRequest(new { error = "AccountId must match format ACC-XXXXX (5 digits)." });

        return Ok(service.GetBalance(accountId));
    }

    [HttpGet("{accountId}/summary")]
    public IActionResult GetSummary(string accountId)
    {
        if (!AccountPattern.IsMatch(accountId))
            return BadRequest(new { error = "AccountId must match format ACC-XXXXX (5 digits)." });

        return Ok(service.GetSummary(accountId));
    }
}
