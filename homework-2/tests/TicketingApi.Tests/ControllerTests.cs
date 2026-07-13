using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketingApi.Controllers;
using TicketingApi.Models;
using TicketingApi.Services;
using Xunit;

namespace TicketingApi.Tests;

public class ControllerTests
{
    [Fact]
    public async Task TicketsController_CreateTicket_ReturnsCreatedResult()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);
        var request = new CreateTicketRequest("Test", "Desc", "User", "user@test.com");

        // Act
        var result = await controller.CreateTicket(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult!.Value.Should().BeOfType<Ticket>();
    }

    [Fact]
    public async Task TicketsController_CreateTicket_InvalidEmail_ReturnsBadRequest()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);
        var request = new CreateTicketRequest("Test", "Desc", "User", "invalid-email");

        // Act
        var result = await controller.CreateTicket(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task TicketsController_CreateTicket_WithAutoClassify_ClassifiesTicket()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);
        var request = new CreateTicketRequest("Urgent billing error", "Payment issue", "User", "user@test.com", AutoClassify: true);

        // Act
        var result = await controller.CreateTicket(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
        var ticket = (result.Result as CreatedAtActionResult)!.Value as Ticket;
        ticket!.AutoClassified.Should().BeTrue();
        ticket.ClassificationConfidence.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task TicketsController_GetTicket_ExistingId_ReturnsOk()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);
        var ticket = await ticketService.CreateTicketAsync(new CreateTicketRequest("T", "D", "N", "e@t.com"));

        // Act
        var result = await controller.GetTicket(ticket.Id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task TicketsController_GetTicket_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);

        // Act
        var result = await controller.GetTicket(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task TicketsController_GetTickets_ReturnsOkWithList()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);
        await ticketService.CreateTicketAsync(new CreateTicketRequest("T1", "D1", "N1", "e1@t.com"));
        await ticketService.CreateTicketAsync(new CreateTicketRequest("T2", "D2", "N2", "e2@t.com"));

        // Act
        var result = await controller.GetTickets();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var tickets = okResult!.Value as List<Ticket>;
        tickets.Should().HaveCount(2);
    }

    [Fact]
    public async Task TicketsController_GetTickets_WithFilters_ReturnsFilteredList()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);
        await ticketService.CreateTicketAsync(new CreateTicketRequest("T1", "D1", "N1", "e1@t.com", Priority: TicketPriority.High));
        await ticketService.CreateTicketAsync(new CreateTicketRequest("T2", "D2", "N2", "e2@t.com", Priority: TicketPriority.Low));

        // Act
        var result = await controller.GetTickets(priority: TicketPriority.High);

        // Assert
        var okResult = result.Result as OkObjectResult;
        var tickets = okResult!.Value as List<Ticket>;
        tickets.Should().HaveCount(1);
    }

    [Fact]
    public async Task TicketsController_UpdateTicket_ValidRequest_ReturnsOk()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);
        var ticket = await ticketService.CreateTicketAsync(new CreateTicketRequest("T", "D", "N", "e@t.com"));

        // Act
        var result = await controller.UpdateTicket(ticket.Id, new UpdateTicketRequest(Subject: "Updated"));

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var updated = (result.Result as OkObjectResult)!.Value as Ticket;
        updated!.Subject.Should().Be("Updated");
    }

    [Fact]
    public async Task TicketsController_UpdateTicket_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);

        // Act
        var result = await controller.UpdateTicket(999, new UpdateTicketRequest(Subject: "Test"));

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task TicketsController_UpdateTicket_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);
        var ticket = await ticketService.CreateTicketAsync(new CreateTicketRequest("T", "D", "N", "e@t.com"));

        // Act
        var result = await controller.UpdateTicket(ticket.Id, new UpdateTicketRequest(Subject: new string('a', 201)));

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task TicketsController_DeleteTicket_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);
        var ticket = await ticketService.CreateTicketAsync(new CreateTicketRequest("T", "D", "N", "e@t.com"));

        // Act
        var result = await controller.DeleteTicket(ticket.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task TicketsController_DeleteTicket_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);

        // Act
        var result = await controller.DeleteTicket(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task TicketsController_AutoClassify_ExistingTicket_ReturnsOkWithClassification()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);
        var ticket = await ticketService.CreateTicketAsync(new CreateTicketRequest("Urgent error", "Critical bug", "User", "user@test.com"));

        // Act
        var result = await controller.AutoClassifyTicket(ticket.Id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var classified = (result.Result as OkObjectResult)!.Value as Ticket;
        classified!.AutoClassified.Should().BeTrue();
        classified.ClassificationConfidence.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task TicketsController_AutoClassify_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var ticketService = new TicketService();
        var classificationService = new ClassificationService();
        var controller = new TicketsController(ticketService, classificationService);

        // Act
        var result = await controller.AutoClassifyTicket(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ImportController_ImportTickets_CsvFile_ReturnsOkWithSummary()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var controller = new ImportController(importService);
        
        var csv = "subject,description,customer_name,customer_email\nTest,Desc,User,user@test.com";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.csv")
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/csv"
        };

        // Act
        var result = await controller.ImportTickets(formFile);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var summary = (result.Result as OkObjectResult)!.Value as ImportSummary;
        summary!.SuccessCount.Should().Be(1);
    }

    [Fact]
    public async Task ImportController_ImportTickets_NoFile_ReturnsBadRequest()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var controller = new ImportController(importService);

        // Act
        var result = await controller.ImportTickets(null!);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ImportController_ImportTickets_UnsupportedFormat_ReturnsBadRequest()
    {
        // Arrange
        var ticketService = new TicketService();
        var importService = new ImportService(ticketService);
        var controller = new ImportController(importService);
        
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("data"));
        var formFile = new FormFile(stream, 0, stream.Length, "file", "test.txt");

        // Act
        var result = await controller.ImportTickets(formFile);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }
}
