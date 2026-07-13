using TicketingApi.Models;

namespace TicketingApi.Services;

public class ClassificationService : IClassificationService
{
    private static readonly Dictionary<string, TicketCategory> CategoryKeywords = new()
    {
        ["login"] = TicketCategory.Technical,
        ["password"] = TicketCategory.Account,
        ["reset"] = TicketCategory.Account,
        ["error"] = TicketCategory.Technical,
        ["bug"] = TicketCategory.Technical,
        ["crash"] = TicketCategory.Technical,
        ["slow"] = TicketCategory.Technical,
        ["performance"] = TicketCategory.Technical,
        ["payment"] = TicketCategory.Billing,
        ["charge"] = TicketCategory.Billing,
        ["invoice"] = TicketCategory.Billing,
        ["refund"] = TicketCategory.Billing,
        ["subscription"] = TicketCategory.Billing,
        ["bill"] = TicketCategory.Billing,
        ["account"] = TicketCategory.Account,
        ["profile"] = TicketCategory.Account,
        ["settings"] = TicketCategory.Account,
        ["delete"] = TicketCategory.Account
    };

    private static readonly Dictionary<string, TicketPriority> PriorityKeywords = new()
    {
        ["urgent"] = TicketPriority.Critical,
        ["critical"] = TicketPriority.Critical,
        ["asap"] = TicketPriority.Critical,
        ["immediately"] = TicketPriority.Critical,
        ["down"] = TicketPriority.Critical,
        ["outage"] = TicketPriority.Critical,
        ["important"] = TicketPriority.High,
        ["high"] = TicketPriority.High,
        ["soon"] = TicketPriority.High,
        ["blocking"] = TicketPriority.High,
        ["question"] = TicketPriority.Low,
        ["help"] = TicketPriority.Low,
        ["how"] = TicketPriority.Low
    };

    public Task<ClassificationResult> ClassifyTicketAsync(string subject, string description)
    {
        var combinedText = $"{subject} {description}".ToLowerInvariant();
        
        var categoryMatches = new Dictionary<TicketCategory, int>();
        var priorityMatches = new Dictionary<TicketPriority, int>();

        foreach (var (keyword, category) in CategoryKeywords)
        {
            var count = CountOccurrences(combinedText, keyword);
            if (count > 0)
            {
                categoryMatches[category] = categoryMatches.GetValueOrDefault(category, 0) + count;
            }
        }

        foreach (var (keyword, priority) in PriorityKeywords)
        {
            var count = CountOccurrences(combinedText, keyword);
            if (count > 0)
            {
                priorityMatches[priority] = priorityMatches.GetValueOrDefault(priority, 0) + count;
            }
        }

        var suggestedCategory = categoryMatches.Any() 
            ? categoryMatches.OrderByDescending(x => x.Value).First().Key 
            : TicketCategory.General;

        var suggestedPriority = priorityMatches.Any()
            ? priorityMatches.OrderByDescending(x => x.Value).First().Key
            : TicketPriority.Medium;

        var totalKeywords = categoryMatches.Values.Sum() + priorityMatches.Values.Sum();
        var confidence = Math.Min(1.0, totalKeywords * 0.2); // Cap at 1.0

        var result = new ClassificationResult(
            suggestedCategory,
            suggestedPriority,
            confidence
        );

        return Task.FromResult(result);
    }

    private static int CountOccurrences(string text, string keyword)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(keyword, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += keyword.Length;
        }
        return count;
    }
}
