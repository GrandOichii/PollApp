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

    // [Fact]
    // public async Task Users_Register_ReturnsSuccess() {
    //     // Arrange
    //     var client = _factory.CreateClient();

    //     // Act
    //     var 

    //     // Assert
    // }
}
