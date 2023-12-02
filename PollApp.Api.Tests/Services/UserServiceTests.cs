using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PollApp.Api.Controllers;
using PollApp.Api.Data;
using PollApp.Api.Dtos;
using PollApp.Api.Models;
using PollApp.Api.Services;

namespace PollApp.Api.Api.Tests;

// Test db functionality here

public class UserServiceTests {
    private IMapper _mapper;
    private IConfiguration _configuration;
    public UserServiceTests() {
        var mC = new MapperConfiguration(cfg => {
            cfg.AddProfile(new AutoMapperProfile());
        });
        _mapper = new Mapper(mC);

        
        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
    }

    private int _userCount = 3;

    private async Task<DataContext> GetDataContext() {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var ctx = new DataContext(options);
        ctx.Database.EnsureCreated();

        for (int i = 0; i < _userCount; i++) {
            ctx.Users.Add(new User() {
                Username = "User" + (i+1),
                PasswordHash =  BCrypt.Net.BCrypt.HashPassword("Pass" + (i+1))
            });
        }
        await ctx.SaveChangesAsync();

        return ctx;
    }


    [Fact]
    public async void UserService_GetAll_ReturnsSuccess() {
        // Arrange
        var ctx = await GetDataContext();
        var userService = new UserService(_configuration, _mapper, ctx);

        // Act
        var result = await userService.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Count().Should().Be(_userCount);
    }

    [Theory]
    [InlineData("User1"), InlineData("User2"), InlineData("User3")]
    public async void UserService_ByUsername_ReturnsSuccess(string username) {
        // Arrange
        var ctx = await GetDataContext();
        var userService = new UserService(_configuration, _mapper, ctx);

        // Act
        var result = await userService.ByUsername(username);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be(username);
    }

    [Fact]
    public async void UserService_ByUsername_ThrowsException() {
        // Arrange
        var username = "non existant user";
        var ctx = await GetDataContext();
        var userService = new UserService(_configuration, _mapper, ctx);

        // Act
        var act = () => userService.ByUsername(username);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async void UserService_Register_ReturnsSuccess() {
        // Arrange
        var ctx = await GetDataContext();
        var userService = new UserService(_configuration, _mapper, ctx);
        
        var username = "new user";
        var newUser = new UserDto() {
            Username = username,
            Password = "password"
        };

        // Act
        var result = await userService.Register(newUser);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be(username);
    }

    [Fact]
    public async void UserService_Register_ThrowsException() {
        // Arrange
        var ctx = await GetDataContext();
        var userService = new UserService(_configuration, _mapper, ctx);
        
        var username = "User1";
        var newUser = new UserDto() {
            Username = username,
            Password = "password"
        };

        // Act
        var act = () => userService.Register(newUser);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async void UserService_Login_ReturnsSuccess() {
        // Arrange
        var ctx = await GetDataContext();
        var userService = new UserService(_configuration, _mapper, ctx);
        var user = new UserDto() {
            Username = "User1",
            Password =  "Pass1"
        };

        // Act
        var result = await userService.Login(user);

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async void UserService_Login_NonexistantUser() {
        // Arrange
        var ctx = await GetDataContext();
        var userService = new UserService(_configuration, _mapper, ctx);
        var user = new UserDto() {
            Username = "non-existant user",
            Password =  "Pass1"
        };

        // Act
        var act = () => userService.Login(user);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }
    [Fact]
    public async void UserService_Login_IncorrectPassword() {
        // Arrange
        var ctx = await GetDataContext();
        var userService = new UserService(_configuration, _mapper, ctx);
        var user = new UserDto() {
            Username = "User1",
            Password =  "incorrect password"
        };

        // Act
        var act = () => userService.Login(user);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

}