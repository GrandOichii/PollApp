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

public class UserControllerTests {
    private UserController _userController;
    private IUserService _userService;

    public UserControllerTests() {
        _userService = A.Fake<IUserService>();

        _userController = new UserController(_userService);
    }

    [Fact]
    public async void UserController_All_ReturnsSuccess() {
        // Arrange
        var users = A.Fake<IEnumerable<GetUserDto>>();
        A.CallTo(() => _userService.GetAll()).Returns(users);

        // Act
        var result = await _userController.GetAll();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async void UserController_Register_ReturnsSuccess() {
        // Arrange
        var user = A.Fake<UserDto>();
        var resultUser = A.Fake<GetUserDto>();

        A.CallTo(() => _userService.Register(user)).Returns(resultUser);
        
        // Act
        var result = await _userController.Register(user);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async void UserController_Register_ReturnsFailed() {
        // Arrange
        var user = A.Fake<UserDto>();
        A.CallTo(() => _userService.Register(user)).Throws<Exception>();

        // Act
        var result = await _userController.Register(user);

        // Assert
        result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async void UserController_Login_ReturnsSuccess() {
        // Arrange
        var user = A.Fake<UserDto>();
        A.CallTo(() => _userService.Login(user)).Returns("jwt");

        // Act
        var result = await _userController.Login(user);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async void UserController_Login_ReturnsFailed() {
        var user = A.Fake<UserDto>();
        A.CallTo(() => _userService.Login(user)).Throws<Exception>();

        // Act
        var result = await _userController.Login(user);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }


}