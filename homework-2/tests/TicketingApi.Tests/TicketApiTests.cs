using FluentAssertions;
using TicketingApi.Models;
using TicketingApi.Services;
using Xunit;

namespace TicketingApi.Tests;

public class TicketApiTests
{
    [Fact]
    public async Task CreateTicket_ValidRequest_ReturnsCreatedTicket()
    {
        // Arrange
        var service = new TicketService();
        var request = new CreateTicketRequest(
            "Login issue",
            "Cannot log in to my account",
            "John Doe",
            "john@example.com"
        );

        // Act
        var ticket = await service.CreateTicketAsync(request);

        // Assert
        ticket.Should().NotBeNull();
        ticket.Id.Should().BeGreaterThan(0);
        ticket.Subject.Should().Be("Login issue");
        ticket.Status.Should().Be(TicketStatus.Open);
        ticket.Priority.Should().Be(TicketPriority.Medium);
        ticket.Category.Should().Be(TicketCategory.General);
    }

    [Fact]
    public async Task CreateTicket_WithPriorityAndCategory_UsesProvidedValues()
    {
        // Arrange
        var service = new TicketService();
        var request = new CreateTicketRequest(
            "Urgent billing issue",
            "Was charged twice",
            "Jane Smith",
            "jane@example.com",
            Priority: TicketPriority.High,
            Category: TicketCategory.Billing
        );

        // Act
        var ticket = await service.CreateTicketAsync(request);

        // Assert
        ticket.Priority.Should().Be(TicketPriority.High);
        ticket.Category.Should().Be(TicketCategory.Billing);
    }

    [Fact]
    public async Task GetTicketById_ExistingId_ReturnsTicket()
    {
        // Arrange
        var service = new TicketService();
        var request = new CreateTicketRequest("Test", "Description", "User", "user@test.com");
        var created = await service.CreateTicketAsync(request);

        // Act
        var ticket = await service.GetTicketByIdAsync(created.Id);

        // Assert
        ticket.Should().NotBeNull();
        ticket!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task GetTicketById_NonExistingId_ReturnsNull()
    {
        // Arrange
        var service = new TicketService();

        // Act
        var ticket = await service.GetTicketByIdAsync(999);

        // Assert
        ticket.Should().BeNull();
    }

    [Fact]
    public async Task GetTickets_NoFilter_ReturnsAllTickets()
    {
        // Arrange
        var service = new TicketService();
        await service.CreateTicketAsync(new CreateTicketRequest("Test 1", "Desc 1", "User 1", "user1@test.com"));
        await service.CreateTicketAsync(new CreateTicketRequest("Test 2", "Desc 2", "User 2", "user2@test.com"));

        // Act
        var tickets = await service.GetTicketsAsync();

        // Assert
        tickets.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTickets_FilterByStatus_ReturnsMatchingTickets()
    {
        // Arrange
        var service = new TicketService();
        var ticket1 = await service.CreateTicketAsync(new CreateTicketRequest("T1", "D1", "U1", "u1@test.com"));
        await service.CreateTicketAsync(new CreateTicketRequest("T2", "D2", "U2", "u2@test.com"));
        await service.UpdateTicketAsync(ticket1.Id, new UpdateTicketRequest(Status: TicketStatus.Resolved));

        // Act
        var tickets = await service.GetTicketsAsync(new TicketFilter(Status: TicketStatus.Resolved));

        // Assert
        tickets.Should().HaveCount(1);
        tickets[0].Status.Should().Be(TicketStatus.Resolved);
    }

    [Fact]
    public async Task UpdateTicket_ValidRequest_UpdatesTicket()
    {
        // Arrange
        var service = new TicketService();
        var ticket = await service.CreateTicketAsync(new CreateTicketRequest("Old", "Old desc", "User", "user@test.com"));

        // Act
        var updated = await service.UpdateTicketAsync(ticket.Id, new UpdateTicketRequest(Subject: "New Subject", Status: TicketStatus.InProgress));

        // Assert
        updated.Should().NotBeNull();
        updated!.Subject.Should().Be("New Subject");
        updated.Status.Should().Be(TicketStatus.InProgress);
    }

    [Fact]
    public async Task UpdateTicket_NonExistingId_ReturnsNull()
    {
        // Arrange
        var service = new TicketService();

        // Act
        var updated = await service.UpdateTicketAsync(999, new UpdateTicketRequest(Subject: "Test"));

        // Assert
        updated.Should().BeNull();
    }

    [Fact]
    public async Task UpdateTicket_ResolvedStatus_SetsResolvedAt()
    {
        // Arrange
        var service = new TicketService();
        var ticket = await service.CreateTicketAsync(new CreateTicketRequest("Test", "Desc", "User", "user@test.com"));

        // Act
        var updated = await service.UpdateTicketAsync(ticket.Id, new UpdateTicketRequest(Status: TicketStatus.Resolved));

        // Assert
        updated!.ResolvedAt.Should().NotBeNull();
        updated.ResolvedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task DeleteTicket_ExistingId_ReturnsTrue()
    {
        // Arrange
        var service = new TicketService();
        var ticket = await service.CreateTicketAsync(new CreateTicketRequest("Test", "Desc", "User", "user@test.com"));

        // Act
        var deleted = await service.DeleteTicketAsync(ticket.Id);

        // Assert
        deleted.Should().BeTrue();
        var retrieved = await service.GetTicketByIdAsync(ticket.Id);
        retrieved.Should().BeNull();
    }

    [Fact]
    public async Task DeleteTicket_NonExistingId_ReturnsFalse()
    {
        // Arrange
        var service = new TicketService();

        // Act
        var deleted = await service.DeleteTicketAsync(999);

        // Assert
        deleted.Should().BeFalse();
    }
}
