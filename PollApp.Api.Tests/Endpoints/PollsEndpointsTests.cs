using System.Net.Http.Json;
using System.Text;
using FakeItEasy;
using FluentAssertions;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PollApp.Api.Dtos;
using PollApp.Api.Models;
using PollApp.Api.Services;
using PollApp.Api.Controllers;
using Xunit.Abstractions;

namespace PollApp.Api.Api.Tests;

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

    private async Task<string> GetJwtToken(HttpClient client) {
        var result = await client.PostAsync("/api/Users/login", JsonContent.Create(new UserDto {
            Username = "User1",
            Password = "Pass1"
        }));
        return await result.Content.ReadAsStringAsync();
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

    [Fact]
    public async Task Polls_AddPoll_ReturnsSuccess() {
        // TODO change to admin only

        // Arrange
        var client = _factory.CreateClient();
        var token = await GetJwtToken(client);

        client.SetBearerToken(token);

        var prevCount = (await client.GetFromJsonAsync<IEnumerable<GetPollDto>>("/api/Polls"))!.Count();
        var newPoll = new AddPollDto {
            Title = "new poll",
            Text = "new poll text",
            Options = new() {
                new() {
                    Text = "new poll option 1"
                },
                new() {
                    Text = "new poll option 2"
                },
            }
        };
        // Act
        var result = await client.PostAsync("/api/Polls", JsonContent.Create(newPoll));
        var newPolls = await result.Content.ReadFromJsonAsync<IEnumerable<GetPollDto>>();

        // Assert
        result.Should().BeSuccessful();
        newPolls!.Count().Should().BeGreaterThan(prevCount);
    }

    [Fact]
    public async Task Polls_AddPoll_ReturnsFailed() {
        // TODO change to admin only

        // Arrange
        var client = _factory.CreateClient();
        var token = await GetJwtToken(client);

        client.SetBearerToken(token);

        var newPoll = new AddPollDto {
            Title = "new poll",
            Text = "new poll text",
            Options = new()
        };
        // Act
        var result = await client.PostAsync("/api/Polls", JsonContent.Create(newPoll));

        // Assert
        result.Should().HaveClientError();
    }

    [Fact]
    public async Task Polls_Vote_ReturnsSuccess() 
    {
        // Arrange
        var client = _factory.CreateClient();
        var token = await GetJwtToken(client);

        client.SetBearerToken(token);

        var pollID = 1;
        var option = "Option1";
        var prevOptionVotes = (await client.GetFromJsonAsync<GetPollDto>("/api/Polls/" + pollID))!.Options[0].VotedUsers.Count;

        // Act
        var result = await client.PutAsync("/api/Polls/vote/", JsonContent.Create(new Vote {
            PollID = pollID,
            Option = option
        }));

        // Assert
        result.Should().BeSuccessful();
        var poll = await result.Content.ReadFromJsonAsync<GetPollDto>();

        poll!.Options[0].VotedUsers.Count.Should().BeGreaterThan(prevOptionVotes);
    }

    [Fact]
    public async Task Polls_Vote_IncorrectOption() {
        // Arrange
        var client = _factory.CreateClient();
        var token = await GetJwtToken(client);

        client.SetBearerToken(token);

        var pollID = 1;
        var option = "non-existant option";

        // Act
        var result = await client.PutAsync("/api/Polls/vote/", JsonContent.Create(new Vote {
            PollID = pollID,
            Option = option
        }));


        // Assert
        result.Should().HaveClientError();
    }

    [Theory]
    [InlineData("Option1"), InlineData("Option2")]
    public async Task Polls_Vote_AlreadyVoted(string option) {
        // Arrange
        var client = _factory.CreateClient();
        var token = await GetJwtToken(client);

        client.SetBearerToken(token);

        var pollID = 1;

        // Act
        
        // first vote
        await client.PutAsync("/api/Polls/vote/", JsonContent.Create(new Vote {
            PollID = pollID,
            Option = "Option1"
        }));

        // second vote
        var result = await client.PutAsync("/api/Polls/vote/", JsonContent.Create(new Vote {
            PollID = pollID,
            Option = option
        }));

        // Assert
        result.Should().HaveClientError();
    }
    
}
