using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using WebApiTutorial.Dtos;

namespace WebApiTutorial.Api.Tests;

// These are integration tests

public class PollsEndpointTests 
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PollsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
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
}
