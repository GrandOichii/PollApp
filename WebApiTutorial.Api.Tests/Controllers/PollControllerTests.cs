using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApiTutorial.Controllers;
using WebApiTutorial.Dtos;
using WebApiTutorial.Models;
using WebApiTutorial.Services;

namespace WebApiTutorial.Api.Tests;

// Test controllers here

public class PollControllerTests {
    private PollController _pollController;
    private IPollService _pollService;

    public PollControllerTests() {
        _pollService = A.Fake<IPollService>();

        _pollController = new PollController(_pollService);
    }

    [Fact]
    public async void PollController_All_ReturnsSuccess() {
        // Arrange
        var polls = A.Fake<IEnumerable<GetPollDto>>();
        A.CallTo(() => _pollService.GetAll()).Returns(polls);

        // Act
        var result = await _pollController.All();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async void PollController_ById_ReturnsSuccess() {
        // Arrange
        var id = 1;
        var poll = A.Fake<GetPollDto>();

        A.CallTo(() => _pollService.ByID(id)).Returns(poll);
        // Act

        var result = await _pollController.ByID(id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async void PollController_ById_ReturnsFailed() {
        // Arrange
        var id = 2;
        A.CallTo(() => _pollService.ByID(id)).Throws<Exception>();
        
        // Act
        var result = await _pollController.ByID(id);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    // TODO add authorized methods
}