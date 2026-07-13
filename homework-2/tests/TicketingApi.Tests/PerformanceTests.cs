using System.Diagnostics;
using System.Text;
using FluentAssertions;
using TicketingApi.Models;
using TicketingApi.Services;
using Xunit;

namespace TicketingApi.Tests;

public class PerformanceTests
{
    [Fact]
    public async Task Performance_Create1000Tickets_CompletesQuickly()
    {
        // Arrange
        var ticketService = new TicketService();
        var sw = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            await ticketService.CreateTicketAsync(new CreateTicketRequest(
                $"Ticket {i}",
                $"Description {i}",
                $"User {i}",
                $"user{i}@test.com"
            ));
        }
        sw.Stop();

        // Assert
        sw.ElapsedMilliseconds.Should().BeLessThan(5000); // Should complete in under 5 seconds
        var tickets = await ticketService.GetTicketsAsync();
        tickets.Should().HaveCount(1000);
    }

    [Fact]
    public async Task Performance_FilterLargeDataset_FastExecution()
    {
        // Arrange
        var ticketService = new TicketService();
        for (int i = 0; i < 500; i++)
        {
            await ticketService.CreateTicketAsync(new CreateTicketRequest(
                $"Ticket {i}",
                "Description",
                "User",
                $"user{i}@test.com",
                Priority: i % 2 == 0 ? TicketPriority.High : TicketPriority.Low,
                Category: i % 3 == 0 ? TicketCategory.Billing : TicketCategory.Technical
            ));
        }

        var sw = Stopwatch.StartNew();

        // Act
        var highPriority = await ticketService.GetTicketsAsync(new TicketFilter(Priority: TicketPriority.High));
        sw.Stop();

        // Assert
        sw.ElapsedMilliseconds.Should().BeLessThan(500);
        highPriority.Should().HaveCount(250);
    }

    [Fact]
    public async Task Performance_BulkCsvImport_HandlesLargeFiles()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var sb = new StringBuilder();
        sb.AppendLine("subject,description,customer_name,customer_email");
        for (int i = 0; i < 100; i++)
        {
            sb.AppendLine($"Subject {i},Description {i},User {i},user{i}@test.com");
        }
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        var sw = Stopwatch.StartNew();

        // Act
        var summary = await importService.ImportFromCsvAsync(stream);
        sw.Stop();

        // Assert
        sw.ElapsedMilliseconds.Should().BeLessThan(3000);
        summary.SuccessCount.Should().Be(100);
    }

    [Fact]
    public async Task Performance_ConcurrentReads_ThreadSafe()
    {
        // Arrange
        var ticketService = new TicketService();
        for (int i = 0; i < 50; i++)
        {
            await ticketService.CreateTicketAsync(new CreateTicketRequest($"T{i}", "D", "N", $"e{i}@t.com"));
        }

        // Act
        var tasks = Enumerable.Range(0, 100).Select(_ => ticketService.GetTicketsAsync()).ToArray();
        await Task.WhenAll(tasks);

        // Assert
        tasks.All(t => t.Result.Count == 50).Should().BeTrue();
    }

    [Fact]
    public async Task Performance_Classification_FastProcessing()
    {
        // Arrange
        var classificationService = new ClassificationService();
        var sw = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            await classificationService.ClassifyTicketAsync("Urgent billing error", "Critical payment issue needs immediate attention");
        }
        sw.Stop();

        // Assert
        sw.ElapsedMilliseconds.Should().BeLessThan(2000);
    }
}
