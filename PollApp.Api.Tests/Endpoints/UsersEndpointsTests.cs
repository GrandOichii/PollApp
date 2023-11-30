using System.Net.Http.Json;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PollApp.Api.Dtos;
using PollApp.Api.Models;
using PollApp.Api.Services;

namespace PollApp.Api.Api.Tests;

// These are integration tests
// Test API endpoints here
// TODO replace mock classes with in memory DBs?

public class UsersEndpointTests 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    // private int _pollsCount;

    public UsersEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder => {
            builder.ConfigureServices(services => {
                services.AddSingleton<IUserService, UserServiceMock>();
            });
        });
    }

    [Fact]
    public async Task Users_GetAll_ReturnsSuccess() {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var users = await client.GetFromJsonAsync<IEnumerable<GetUserDto>>("/api/Users");
    
        // Assert
        users.Should().NotBeNull();
    }

    [Fact]
    public async Task Users_Register_ReturnsSuccess() {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var result = await client.PostAsync("/api/Users/register", JsonContent.Create(new UserDto {
            Username = "new user",
            Password = "new password"
        }));

        // Assert
        result.Should().BeSuccessful();

    }

    [Fact]
    public async Task Users_Register_ReturnsFailed() {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var result = await client.PostAsync("/api/Users/register", JsonContent.Create(new UserDto {
            Username = "User1",
            Password = "password"
        }));

        // Assert
        result.Should().HaveClientError();
    }

    [Fact]
    public async Task Users_Login_ReturnsSuccess() {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var result = await client.PostAsync("/api/Users/login", JsonContent.Create(new UserDto {
            Username = "User1",
            Password = "Pass1"
        }));

        // Assert
        result.Should().BeSuccessful();

    }

    [Fact]
    public async Task User_Login_ReturnsFailed() {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var result = await client.PostAsync("/api/Users/login", JsonContent.Create(new UserDto {
            Username = "non existant user",
            Password = "password"
        }));

        // Assert
        result.Should().HaveClientError();
    }
}
