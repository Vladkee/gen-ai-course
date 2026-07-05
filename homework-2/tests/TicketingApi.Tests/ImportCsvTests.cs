using System.Text;
using FluentAssertions;
using TicketingApi.Services;
using Xunit;

namespace TicketingApi.Tests;

public class ImportCsvTests
{
    [Fact]
    public async Task ImportCsv_ValidData_ImportsSuccessfully()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var csv = """
            subject,description,customer_name,customer_email,priority,category
            Login issue,Cannot login,John Doe,john@example.com,High,Technical
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        // Act
        var summary = await importService.ImportFromCsvAsync(stream);

        // Assert
        summary.TotalRows.Should().Be(1);
        summary.SuccessCount.Should().Be(1);
        summary.ErrorCount.Should().Be(0);
        var tickets = await ticketService.GetTicketsAsync();
        tickets.Should().HaveCount(1);
    }

    [Fact]
    public async Task ImportCsv_MultipleRows_ImportsAll()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var csv = """
            subject,description,customer_name,customer_email
            Issue 1,Description 1,User 1,user1@test.com
            Issue 2,Description 2,User 2,user2@test.com
            Issue 3,Description 3,User 3,user3@test.com
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        // Act
        var summary = await importService.ImportFromCsvAsync(stream);

        // Assert
        summary.SuccessCount.Should().Be(3);
        summary.TotalRows.Should().Be(3);
    }

    [Fact]
    public async Task ImportCsv_InvalidEmail_ReturnsError()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var csv = """
            subject,description,customer_name,customer_email
            Issue,Desc,User,invalid-email
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        // Act
        var summary = await importService.ImportFromCsvAsync(stream);

        // Assert
        summary.ErrorCount.Should().Be(1);
        summary.SuccessCount.Should().Be(0);
        summary.Errors[0].Field.Should().Be("customerEmail");
    }

    [Fact]
    public async Task ImportCsv_MissingRequiredField_ReturnsError()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var csv = """
            subject,description,customer_name,customer_email
            ,Description,User,user@test.com
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        // Act
        var summary = await importService.ImportFromCsvAsync(stream);

        // Assert
        summary.ErrorCount.Should().Be(1);
        summary.Errors[0].ErrorMessage.Should().Contain("Subject is required");
    }

    [Fact]
    public async Task ImportCsv_MixedValidAndInvalid_ReportsCorrectly()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var csv = """
            subject,description,customer_name,customer_email
            Valid,Description,User,user@test.com
            ,Missing subject,User 2,user2@test.com
            Valid 2,Description 2,User 3,user3@test.com
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        // Act
        var summary = await importService.ImportFromCsvAsync(stream);

        // Assert
        summary.TotalRows.Should().Be(3);
        summary.SuccessCount.Should().Be(2);
        summary.ErrorCount.Should().Be(1);
        summary.Errors[0].RowNumber.Should().Be(3); // Row 2 is the header, row 3 is first data row with error
    }

    [Fact]
    public async Task ImportCsv_InvalidFormat_ReturnsFileError()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var invalidCsv = "not,valid,csv\nwithout\nproper\nstructure";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(invalidCsv));

        // Act
        var summary = await importService.ImportFromCsvAsync(stream);

        // Assert
        summary.ErrorCount.Should().BeGreaterThan(0);
    }
}
