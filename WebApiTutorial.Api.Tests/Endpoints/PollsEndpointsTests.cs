using System.Net.Http.Json;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WebApiTutorial.Dtos;
using WebApiTutorial.Models;
using WebApiTutorial.Services;

namespace WebApiTutorial.Api.Tests;

// These are integration tests
// Test API endpoints here
// TODO replace mock classes with in memory DBs?

public class PollsEndpointTests 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    // private int _pollsCount;

    public PollsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => {
            builder.ConfigureServices(services => {
                // var pollServiceMock = A.Fake<IPollService>();

                // var polls = new List<GetPollDto>();

                // for (int i = 0; i < 10; i++) {
                //     var poll = new GetPollDto() {
                //         ID = i + 1,
                //         Title = "Poll" + (i+1),
                //         Text = "Text" + (i+1)
                //     };
                //     polls.Add(poll);

                // }

                // _pollsCount = polls.Count;
                // A.CallTo(() => pollServiceMock.GetAll()).Returns(polls);
                services.AddSingleton<IUserService, UserServiceMock>();
                services.AddSingleton<IPollService, PollServiceMock>();
            });
        });
    }

    [Fact]
    public async Task Polls_GetAll_ReturnsSuccess() {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var polls = await client.GetFromJsonAsync<IEnumerable<GetPollDto>>("/api/Polls");
    
        // Assert
        polls.Should().NotBeNull();
    }

    [Theory]
    [InlineData(1), InlineData(2), InlineData(3)]
    public async Task Polls_ByID_ReturnsSuccess(int id) {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var poll = await client.GetFromJsonAsync<GetPollDto>("/api/Polls/" + id);
    
        // Assert
        poll.Should().NotBeNull();
        poll!.ID.Should().Be(id);
    }

    [Fact]
    public async Task Polls_ById_ReturnsFailed() {
        // Arrange
        var client = _factory.CreateClient();

        var id = 0;

        // Act
        var result = await client.GetAsync("/api/Polls/" + id);

        // Assert
        result.Should().HaveClientError();
    }
}
