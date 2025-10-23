namespace Fcg.Tests;

using BCrypt.Net;
using Fcg.Application.DTOs.Auth;
using Fcg.Application.Services;
using Fcg.Data.Repositories.Interface;
using Fcg.Domain;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Linq.Expressions;
using Xunit;


public class AuthenticationTest
{
    private readonly Mock<IUnityOfWork> _unitOfWorkMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthenticationTest()
    {
        _unitOfWorkMock = new Mock<IUnityOfWork>();
        _configurationMock = new Mock<IConfiguration>();

        // Setup configuration mock
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("w4jR9s8Vq2gH1bY7uKxZpQ3tS6vF0nLm+YzB4rT1sU=");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("Pos.Api.Games");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("Pos.Api.Games");

        _authService = new AuthService(_unitOfWorkMock.Object, _configurationMock.Object);
    }


    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnSuccess()
    {
        //Arrange
        var password = "Test@1234";
        var passwordHash = BCrypt.HashPassword(password);

        var user = new User
        {
            Id = 1,
            Name = "Test",
            Email = "test@gmail.com",
            Password = passwordHash,
            Role = UserRole.User
        };

        // Use a senha em texto plano no LoginRequest (não o hash)
        var loginRequest = new LoginRequest(user.Email, password);

        var _userRepoMock = new Mock<IRepository<User>>();

        _userRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { user });

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);

        //Act
        var result = await _authService.LoginUser(loginRequest, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Error.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ReturnFailure()
    {
        //Arrange
        var loginRequest = new LoginRequest("invalidEmail", "Password@1234");

        var _userRepoMock = new Mock<IRepository<User>>();

        _userRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<User>());

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);

        //act
        var result = await _authService.LoginUser(loginRequest, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("Invalid e-mail or password");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnFailure()
    {
        //Arrange
        var correctPassword = "Teste@1234";
        var wrongPassword = "Wrong@1234";

        var passwordHash = BCrypt.HashPassword(correctPassword);

        var user = new User
        {
            Id = 1,
            Name = "Teste",
            Email = "teste@gmail.com",
            Password = passwordHash,
            Role = UserRole.User
        };

        var loginRequest = new LoginRequest(user.Email, wrongPassword);

        var _userRepoMock = new Mock<IRepository<User>>();

        _userRepoMock
            .Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { user });

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);

        //Act

        var result = await _authService.LoginUser(loginRequest, CancellationToken.None);

        //Assert

        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("Invalid e-mail or password");
    }

    [Fact]
    public void GenerateJwtToken_ShouldGenerateValidToken()
    {
        //Arrange
        var userId = 1;
        var email = "Teste@gmail.com";
        var role = "User";

        //Act
        var token = _authService.GenerateJwtToken(userId, email, role);

        //Assert
        token.Should().NotBe(null);
        token.Split('.').Should().HaveCount(3);
    }

}