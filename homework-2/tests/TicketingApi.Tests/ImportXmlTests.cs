using System.Text;
using FluentAssertions;
using TicketingApi.Services;
using Xunit;

namespace TicketingApi.Tests;

public class ImportXmlTests
{
    [Fact]
    public async Task ImportXml_ValidData_ImportsSuccessfully()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var xml = """
            <tickets>
                <ticket>
                    <subject>Login issue</subject>
                    <description>Cannot login</description>
                    <customer_name>John Doe</customer_name>
                    <customer_email>john@example.com</customer_email>
                    <priority>High</priority>
                    <category>Technical</category>
                </ticket>
            </tickets>
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var summary = await importService.ImportFromXmlAsync(stream);

        // Assert
        summary.TotalRows.Should().Be(1);
        summary.SuccessCount.Should().Be(1);
        summary.ErrorCount.Should().Be(0);
    }

    [Fact]
    public async Task ImportXml_MultipleTickets_ImportsAll()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var xml = """
            <tickets>
                <ticket>
                    <subject>Issue 1</subject>
                    <description>Desc 1</description>
                    <customer_name>User 1</customer_name>
                    <customer_email>u1@test.com</customer_email>
                </ticket>
                <ticket>
                    <subject>Issue 2</subject>
                    <description>Desc 2</description>
                    <customer_name>User 2</customer_name>
                    <customer_email>u2@test.com</customer_email>
                </ticket>
            </tickets>
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var summary = await importService.ImportFromXmlAsync(stream);

        // Assert
        summary.SuccessCount.Should().Be(2);
        summary.TotalRows.Should().Be(2);
    }

    [Fact]
    public async Task ImportXml_InvalidEmail_ReturnsError()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var xml = """
            <tickets>
                <ticket>
                    <subject>Test</subject>
                    <description>Desc</description>
                    <customer_name>User</customer_name>
                    <customer_email>invalid</customer_email>
                </ticket>
            </tickets>
            """;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var summary = await importService.ImportFromXmlAsync(stream);

        // Assert
        summary.ErrorCount.Should().Be(1);
        summary.SuccessCount.Should().Be(0);
    }

    [Fact]
    public async Task ImportXml_MalformedXml_ReturnsError()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var xml = """<tickets><ticket>unclosed""";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var summary = await importService.ImportFromXmlAsync(stream);

        // Assert
        summary.ErrorCount.Should().BeGreaterThan(0);
        summary.Errors[0].Field.Should().Be("file");
    }

    [Fact]
    public async Task ImportXml_EmptyRoot_ReturnsZeroTickets()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var xml = """<tickets></tickets>""";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        // Act
        var summary = await importService.ImportFromXmlAsync(stream);

        // Assert
        summary.TotalRows.Should().Be(0);
        summary.SuccessCount.Should().Be(0);
        summary.ErrorCount.Should().Be(0);
    }
}
