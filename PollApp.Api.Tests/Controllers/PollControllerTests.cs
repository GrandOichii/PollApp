using System.Security.Claims;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PollApp.Api.Controllers;
using PollApp.Api.Dtos;
using PollApp.Api.Models;
using PollApp.Api.Services;

namespace PollApp.Api.Api.Tests;

// Test controllers here

public class PollControllerTests {
    private PollController _pollController;
    private IPollService _pollService;

    public PollControllerTests() {
        _pollService = A.Fake<IPollService>();

        _pollController = new PollController(_pollService);
    }

    private void AddUser(string username) {
          var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new(ClaimTypes.Name, username)
                                   }));

        _pollController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
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

    [Fact]
    public async void PollController_AddPoll_ReturnsSuccess() {
        // Arrange
        var poll = A.Fake<AddPollDto>();
        A.CallTo(() => _pollService.Add(poll)).Returns(A.Fake<IEnumerable<GetPollDto>>());
        
        // Act
        var result = await _pollController.AddPoll(poll);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }
    
    [Fact]
    public async void PollController_AddPoll_ReturnsFailed() {
        // Arrange
        var poll = A.Fake<AddPollDto>();
        A.CallTo(() => _pollService.Add(poll)).Throws<Exception>();
        
        // Act
        var result = await _pollController.AddPoll(poll);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async void PollController_VoteFor_ReturnsSuccess() {
        // Arrange
        var username = "username";
        var pollID = 0;
        var option = "option";
        
        A.CallTo(() => _pollService.VoteFor(username, pollID, option)).Returns(A.Fake<GetPollDto>());
        AddUser(username);

        // Act
        var result = await _pollController.VoteFor(new Vote {
            PollID = 0,
            Option = option
        });

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async void PollController_VoteFor_ReturnsFailed() {
        // Arrange
        var username = "username";
        var pollID = 0;
        var option = "option";
        
        A.CallTo(() => _pollService.VoteFor(username, pollID, option)).Throws<Exception>();
        AddUser(username);

        // Act
        var result = await _pollController.VoteFor(new Vote {
            PollID = 0,
            Option = option
        });

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async void PollController_VoteFor_AlreadyVoted() {
        // Arrange
        var username = "username";
        var pollID = 0;
        var option = "option";
        
        A.CallTo(() => _pollService.VoteFor(username, pollID, option))
            .Returns(A.Fake<GetPollDto>())
            .Once()
            .Then
            .Throws<Exception>();
        AddUser(username);
        var vote = new Vote {
            PollID = 0,
            Option = option
        };

        // Act
        await _pollController.VoteFor(vote);
        var result = await _pollController.VoteFor(vote);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}