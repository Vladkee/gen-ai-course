using FluentAssertions;
using TicketingApi.Models;
using TicketingApi.Validation;
using Xunit;

namespace TicketingApi.Tests;

public class TicketModelTests
{
    [Fact]
    public void TicketValidator_ValidData_ReturnsNoErrors()
    {
        // Act
        var errors = TicketValidator.ValidateCreateRequest(
            "Valid subject",
            "Valid description",
            "John Doe",
            "john@example.com"
        );

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact]
    public void TicketValidator_EmptySubject_ReturnsError()
    {
        // Act
        var errors = TicketValidator.ValidateCreateRequest("", "Desc", "Name", "email@test.com");

        // Assert
        errors.Should().ContainSingle();
        errors[0].ErrorMessage.Should().Contain("Subject is required");
    }

    [Fact]
    public void TicketValidator_SubjectTooLong_ReturnsError()
    {
        // Act
        var errors = TicketValidator.ValidateCreateRequest(new string('a', 201), "Desc", "Name", "email@test.com");

        // Assert
        errors.Should().ContainSingle();
        errors[0].ErrorMessage.Should().Contain("200 characters");
    }

    [Fact]
    public void TicketValidator_InvalidEmail_ReturnsError()
    {
        // Act
        var errors = TicketValidator.ValidateCreateRequest("Subject", "Desc", "Name", "invalid-email");

        // Assert
        errors.Should().ContainSingle();
        errors[0].ErrorMessage.Should().Contain("not valid");
    }

    [Fact]
    public void TicketValidator_DescriptionTooLong_ReturnsError()
    {
        // Act
        var errors = TicketValidator.ValidateCreateRequest("Subject", new string('a', 2001), "Name", "email@test.com");

        // Assert
        errors.Should().ContainSingle();
        errors[0].ErrorMessage.Should().Contain("2000 characters");
    }

    [Fact]
    public void TicketValidator_CustomerNameTooLong_ReturnsError()
    {
        // Act
        var errors = TicketValidator.ValidateCreateRequest("Subject", "Desc", new string('a', 101), "email@test.com");

        // Assert
        errors.Should().ContainSingle();
        errors[0].ErrorMessage.Should().Contain("100 characters");
    }

    [Fact]
    public void TicketValidator_MultipleErrors_ReturnsAll()
    {
        // Act
        var errors = TicketValidator.ValidateCreateRequest("", "", "", "");

        // Assert
        errors.Should().HaveCount(4);
    }

    [Fact]
    public void CreateTicketRequest_DefaultValues_AreCorrect()
    {
        // Act
        var request = new CreateTicketRequest("S", "D", "N", "e@t.com");

        // Assert
        request.Priority.Should().BeNull();
        request.Category.Should().BeNull();
        request.Tags.Should().BeNull();
        request.AutoClassify.Should().BeFalse();
    }

    [Fact]
    public void UpdateTicketRequest_AllOptional_CanBeNull()
    {
        // Act
        var request = new UpdateTicketRequest();

        // Assert
        request.Subject.Should().BeNull();
        request.Status.Should().BeNull();
        request.Priority.Should().BeNull();
    }
}
