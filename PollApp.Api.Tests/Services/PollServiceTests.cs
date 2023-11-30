using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PollApp.Api.Controllers;
using PollApp.Api.Data;
using PollApp.Api.Dtos;
using PollApp.Api.Models;
using PollApp.Api.Services;

namespace PollApp.Api.Api.Tests;

// Test db functionality here

public class PollServiceTests {
    private IMapper _mapper;
    public PollServiceTests() {
        var mC = new MapperConfiguration(cfg => {
            cfg.AddProfile(new AutoMapperProfile());
        });
        _mapper = new Mapper(mC);
    }

    private int _pollCount = 10;

    private async Task<DataContext> GetDataContext() {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new DataContext(options);
        ctx.Database.EnsureCreated();

        for (int i = 0; i < _pollCount; i++) {
            ctx.Polls.Add(new Poll {
                Title = "Poll title " + (i + 1).ToString(),
                Text = "This is the text of the poll with id " + (i + 1).ToString(),
                Options = new() {
                    new() {
                        Text = "Option 1"
                    },
                    new() {
                        Text = "Option 2"
                    }
                }
            });
        }
        ctx.Users.Add(new User {
            Username="user1",
            PasswordHash="passhash"
        });

        await ctx.SaveChangesAsync();

        return ctx;
    }


    [Fact]
    public async void PollService_GetAll_ReturnsSuccess() {
        // Arrange
        var ctx = await GetDataContext();
        var pollService = new PollService(_mapper, ctx);

        // Act
        var result = await pollService.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(_pollCount);
    }

    [Fact]
    public async void PollService_ById_ReturnsSuccess() {
        // Arrange
        var id = 1;
        var ctx = await GetDataContext();
        var pollService = new PollService(_mapper, ctx);

        // Act
        var result = await pollService.ByID(id);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<GetPollDto>();
    }

    [Fact]
    public async void PollService_ById_ThrowsException() {
        // Arrange
        var id = 0;
        var ctx = await GetDataContext();
        var pollService = new PollService(_mapper, ctx);

        // Act
        var act = () => pollService.ByID(id);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async void PollService_Add_ReturnsSuccess() {
        // Arrange
        var poll = new AddPollDto() {
            Title = "poll title",
            Text = "poll text",
            Options = new() {
                new() {
                    Text = "Option 1"
                },
                new() {
                    Text = "Option 2"
                }
            }
        };
        var ctx = await GetDataContext();

        var pollService = new PollService(_mapper, ctx);
        var prevCount = ctx.Polls.Count();

        // Act
        var result = await pollService.Add(poll);

        // Assert
        prevCount.Should().NotBe(0);
        result.Count().Should().NotBe(prevCount);
    }

    [Fact]
    public async void PollService_Add_ThrowsException() {
        // Arrange
        var poll = new AddPollDto() {
            Title = "poll title",
            Text = "poll text",
            Options = new() {}
        };
        var ctx = await GetDataContext();

        var pollService = new PollService(_mapper, ctx);

        // Act
        var act = () => pollService.Add(poll);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async void PollService_VoteFor_ReturnsSuccess() {
        // Arrange
        var poll = A.Fake<AddPollDto>();
        var ctx = await GetDataContext();

        var pollService = new PollService(_mapper, ctx);
        var prevVoteCount = (await ctx.Polls.FirstAsync()).Options[0].VotedUsers.Count;

        // Act
        var result = await pollService.VoteFor("user1", 1, "Option 1");

        // Assert
        result.Options[0].VotedUsers.Count.Should().BeGreaterThan(prevVoteCount);
    }

    [Fact]
    public async void PollService_VoteFor_IncorrectOption() {
        // Arrange
        var poll = A.Fake<AddPollDto>();
        var ctx = await GetDataContext();

        var pollService = new PollService(_mapper, ctx);
        var prevVoteCount = (await ctx.Polls.FirstAsync()).Options[0].VotedUsers.Count;

        // Act
        var act = () => pollService.VoteFor("user1", 1, "non-existant option");

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Theory]
    [InlineData("Option 1"), InlineData("Option 2")]
    public async void PollService_VoteFor_AlreadyVoted(string option) {
        // Arrange
        var poll = A.Fake<AddPollDto>();
        var ctx = await GetDataContext();

        var pollService = new PollService(_mapper, ctx);

        // Act
        await pollService.VoteFor("user1", 1, "Option 1");
        var act = () => pollService.VoteFor("user1", 1, option);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
}