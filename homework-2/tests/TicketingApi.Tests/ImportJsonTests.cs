using System.Text;
using FluentAssertions;
using TicketingApi.Services;
using Xunit;

namespace TicketingApi.Tests;

public class ImportJsonTests
{
    [Fact]
    public async Task ImportJson_ValidData_ImportsSuccessfully()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var json = """
            [
                {
                    "subject": "Login issue",
                    "description": "Cannot login",
                    "customer_name": "John Doe",
                    "customer_email": "john@example.com",
                    "priority": "High",
                    "category": "Technical"
                }
            ]
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var summary = await importService.ImportFromJsonAsync(stream);

        // Assert
        summary.TotalRows.Should().Be(1);
        summary.SuccessCount.Should().Be(1);
        summary.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task ImportJson_MultipleTickets_ImportsAll()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var json = """
            [
                {"subject": "Issue 1", "description": "Desc 1", "customer_name": "User 1", "customer_email": "u1@test.com"},
                {"subject": "Issue 2", "description": "Desc 2", "customer_name": "User 2", "customer_email": "u2@test.com"}
            ]
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var summary = await importService.ImportFromJsonAsync(stream);

        // Assert
        summary.SuccessCount.Should().Be(2);
        summary.TotalRows.Should().Be(2);
    }

    [Fact]
    public async Task ImportJson_InvalidEmail_ReturnsError()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var json = """
            [{"subject": "Test", "description": "Desc", "customer_name": "User", "customer_email": "invalid"}]
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var summary = await importService.ImportFromJsonAsync(stream);

        // Assert
        summary.ErrorCount.Should().Be(1);
        summary.SuccessCount.Should().Be(0);
    }

    [Fact]
    public async Task ImportJson_NotArray_ReturnsError()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var json = """{"subject": "Test"}""";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var summary = await importService.ImportFromJsonAsync(stream);

        // Assert
        summary.ErrorCount.Should().Be(1);
        summary.Errors[0].ErrorMessage.Should().Contain("array");
    }

    [Fact]
    public async Task ImportJson_MalformedJson_ReturnsError()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var json = """[{invalid json}]""";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));

        // Act
        var summary = await importService.ImportFromJsonAsync(stream);

        // Assert
        summary.ErrorCount.Should().BeGreaterThan(0);
        summary.Errors[0].Field.Should().Be("file");
    }
}
