using FluentAssertions;
using TicketingApi.Models;
using TicketingApi.Services;
using Xunit;

namespace TicketingApi.Tests;

public class ClassificationTests
{
    [Fact]
    public async Task ClassifyTicket_TechnicalKeywords_SuggestsTechnicalCategory()
    {
        // Arrange
        var service = new ClassificationService();

        // Act
        var result = await service.ClassifyTicketAsync("Login error", "I'm getting a 500 error when trying to login");

        // Assert
        result.SuggestedCategory.Should().Be(TicketCategory.Technical);
        result.Confidence.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ClassifyTicket_BillingKeywords_SuggestsBillingCategory()
    {
        // Arrange
        var service = new ClassificationService();

        // Act
        var result = await service.ClassifyTicketAsync("Payment issue", "I need a refund for my last invoice");

        // Assert
        result.SuggestedCategory.Should().Be(TicketCategory.Billing);
    }

    [Fact]
    public async Task ClassifyTicket_AccountKeywords_SuggestsAccountCategory()
    {
        // Arrange
        var service = new ClassificationService();

        // Act
        var result = await service.ClassifyTicketAsync("Password reset", "Need to reset my account password");

        // Assert
        result.SuggestedCategory.Should().Be(TicketCategory.Account);
    }

    [Fact]
    public async Task ClassifyTicket_UrgentKeywords_SuggestsCriticalPriority()
    {
        // Arrange
        var service = new ClassificationService();

        // Act
        var result = await service.ClassifyTicketAsync("URGENT: System down", "The entire system is down ASAP help needed");

        // Assert
        result.SuggestedPriority.Should().Be(TicketPriority.Critical);
    }

    [Fact]
    public async Task ClassifyTicket_HighKeywords_SuggestsHighPriority()
    {
        // Arrange
        var service = new ClassificationService();

        // Act
        var result = await service.ClassifyTicketAsync("Important issue", "This is blocking my work");

        // Assert
        result.SuggestedPriority.Should().Be(TicketPriority.High);
    }

    [Fact]
    public async Task ClassifyTicket_QuestionKeywords_SuggestsLowPriority()
    {
        // Arrange
        var service = new ClassificationService();

        // Act
        var result = await service.ClassifyTicketAsync("Question", "How do I change my settings?");

        // Assert
        result.SuggestedPriority.Should().Be(TicketPriority.Low);
    }

    [Fact]
    public async Task ClassifyTicket_NoKeywords_SuggestsDefaults()
    {
        // Arrange
        var service = new ClassificationService();

        // Act
        var result = await service.ClassifyTicketAsync("Random", "Some random text");

        // Assert
        result.SuggestedCategory.Should().Be(TicketCategory.General);
        result.SuggestedPriority.Should().Be(TicketPriority.Medium);
        result.Confidence.Should().Be(0);
    }

    [Fact]
    public async Task ClassifyTicket_MultipleKeywords_HigherConfidence()
    {
        // Arrange
        var service = new ClassificationService();

        // Act
        var result = await service.ClassifyTicketAsync(
            "Critical bug in payment system",
            "Urgent! The billing system has a critical error and is down"
        );

        // Assert
        result.Confidence.Should().BeGreaterThan(0.5);
    }

    [Fact]
    public async Task ClassifyTicket_ConfidenceCappedAt1()
    {
        // Arrange
        var service = new ClassificationService();
        var manyKeywords = string.Join(" ", Enumerable.Repeat("urgent critical error bug payment invoice billing asap", 10));

        // Act
        var result = await service.ClassifyTicketAsync(manyKeywords, manyKeywords);

        // Assert
        result.Confidence.Should().BeLessOrEqualTo(1.0);
    }

    [Fact]
    public async Task ClassifyTicket_CaseInsensitive_WorksCorrectly()
    {
        // Arrange
        var service = new ClassificationService();

        // Act
        var result = await service.ClassifyTicketAsync("URGENT ERROR", "CRITICAL BUG IN LOGIN");

        // Assert
        result.SuggestedCategory.Should().Be(TicketCategory.Technical);
        result.SuggestedPriority.Should().Be(TicketPriority.Critical);
    }
}
