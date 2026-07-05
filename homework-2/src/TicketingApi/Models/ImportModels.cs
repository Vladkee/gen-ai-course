namespace TicketingApi.Models;

public record ImportSummary(
    int TotalRows,
    int SuccessCount,
    int ErrorCount,
    List<ImportError> Errors
);

public record ImportError(
    int RowNumber,
    string Field,
    string ErrorMessage
);

public record ClassificationResult(
    TicketCategory SuggestedCategory,
    TicketPriority SuggestedPriority,
    double Confidence
);
