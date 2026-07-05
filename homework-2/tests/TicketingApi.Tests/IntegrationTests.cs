using FluentAssertions;
using TicketingApi.Models;
using TicketingApi.Services;
using Xunit;

namespace TicketingApi.Tests;

public class IntegrationTests
{
    [Fact]
    public async Task EndToEnd_CreateClassifyUpdate_WorksCorrectly()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();

        // Act - Create
        var ticket = await ticketService.CreateTicketAsync(new CreateTicketRequest(
            "Login error urgent",
            "Critical bug in login system",
            "User",
            "user@test.com"
        ));

        // Act - Classify
        var classification = await classificationService.ClassifyTicketAsync(ticket.Subject, ticket.Description);
        await ticketService.UpdateTicketAsync(ticket.Id, new UpdateTicketRequest(
            Category: classification.SuggestedCategory,
            Priority: classification.SuggestedPriority
        ));

        // Act - Update
        var updated = await ticketService.UpdateTicketAsync(ticket.Id, new UpdateTicketRequest(
            Status: TicketStatus.Resolved,
            AssignedTo: "Admin"
        ));

        // Assert
        updated.Should().NotBeNull();
        updated!.Category.Should().Be(TicketCategory.Technical);
        updated.Priority.Should().Be(TicketPriority.Critical);
        updated.Status.Should().Be(TicketStatus.Resolved);
        updated.AssignedTo.Should().Be("Admin");
        updated.ResolvedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task EndToEnd_ImportAndFilter_WorksCorrectly()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var csv = """
            subject,description,customer_name,customer_email,priority,category
            Billing issue,Refund request,User 1,u1@test.com,High,Billing
            Tech problem,Login bug,User 2,u2@test.com,Critical,Technical
            General query,How to use,User 3,u3@test.com,Low,General
            """;
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

        // Act - Import
        var summary = await importService.ImportFromCsvAsync(stream);

        // Act - Filter by category
        var billingTickets = await ticketService.GetTicketsAsync(new TicketFilter(Category: TicketCategory.Billing));
        var criticalTickets = await ticketService.GetTicketsAsync(new TicketFilter(Priority: TicketPriority.Critical));

        // Assert
        summary.SuccessCount.Should().Be(3);
        billingTickets.Should().HaveCount(1);
        billingTickets[0].Subject.Should().Be("Billing issue");
        criticalTickets.Should().HaveCount(1);
        criticalTickets[0].Priority.Should().Be(TicketPriority.Critical);
    }

    [Fact]
    public async Task EndToEnd_CreateWithAutoClassify_AppliesClassification()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();

        // Act
        var ticket = await ticketService.CreateTicketAsync(new CreateTicketRequest(
            "Payment refund urgent",
            "Need immediate refund on my invoice",
            "Customer",
            "customer@test.com",
            AutoClassify: true
        ));

        var classification = await classificationService.ClassifyTicketAsync(ticket.Subject, ticket.Description);

        // Assert (classification should have been applied)
        classification.SuggestedCategory.Should().Be(TicketCategory.Billing);
        classification.SuggestedPriority.Should().Be(TicketPriority.Critical);
    }

    [Fact]
    public async Task EndToEnd_DeleteAndVerify_TicketGone()
    {
        // Arrange
        var ticketService = new TicketService();
        var ticket = await ticketService.CreateTicketAsync(new CreateTicketRequest("T", "D", "N", "e@t.com"));

        // Act
        var deleted = await ticketService.DeleteTicketAsync(ticket.Id);
        var retrieved = await ticketService.GetTicketByIdAsync(ticket.Id);

        // Assert
        deleted.Should().BeTrue();
        retrieved.Should().BeNull();
    }

    [Fact]
    public async Task EndToEnd_DateRangeFilter_WorksCorrectly()
    {
        // Arrange
        var ticketService = new TicketService();
        await ticketService.CreateTicketAsync(new CreateTicketRequest("T1", "D1", "N1", "e1@t.com"));
        await Task.Delay(100);
        var midTime = DateTime.UtcNow;
        await Task.Delay(100);
        await ticketService.CreateTicketAsync(new CreateTicketRequest("T2", "D2", "N2", "e2@t.com"));

        // Act
        var afterMidTime = await ticketService.GetTicketsAsync(new TicketFilter(FromDate: midTime));

        // Assert
        afterMidTime.Should().HaveCount(1);
        afterMidTime[0].Subject.Should().Be("T2");
    }
}
