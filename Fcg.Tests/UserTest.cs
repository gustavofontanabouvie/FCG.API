using Moq;
using Fcg.Domain;
using Fcg.Application.Services;
using Fcg.Data.Repositories.Interface;
using System.Linq.Expressions;
using Fcg.Application.DTOs.User;
using FluentAssertions;

namespace Fcg.Tests;

public class UserTest
{
    private readonly Mock<IUnityOfWork> _unitOfWorkMock;
    private readonly UserService _userService;
    public UserTest()
    {
        _unitOfWorkMock = new Mock<IUnityOfWork>();
        _userService = new UserService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ShouldCreateUserSuccessfully()
    {
        // Arrange
        var newUser = new CreateUserDto("teste", "email@email.com", "Teste@1234");

        var userRepoMock = new Mock<IRepository<User>>();

        userRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        userRepoMock.
            Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken token) => u);

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        //Act
        var result = await _userService.CreateUser(newUser, CancellationToken.None);
        //Assert

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Error.Should().BeNull();

        userRepoMock
            .Verify(r => r.AddAsync(It.Is<User>(u => u.Email == newUser.email && u.Role == UserRole.User),
            It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task CreateUser_WithEmailInUse_ShouldFailure()
    {
        // Arrange
        var newUser = new CreateUserDto("name", "usingemail@emai.com", "Teste@1234");

        var userRepoMock = new Mock<IRepository<User>>();

        userRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);
        // Act
        var result = await _userService.CreateUser(newUser, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("E-mail already in use");
    }

    [Fact]
    public async Task CreateAdminAsync_WithValidData_ShouldCreateAdminUserSuccessfully()
    {
        // Arrange
        var newAdmin = new CreateUserDto("admin", "admin@admin.com", "Teste@123");

        var userRepoMock = new Mock<IRepository<User>>();

        userRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        userRepoMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync((User u, CancellationToken token) => u);

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        // Act
        var result = await _userService.CreateAdminUser(newAdmin, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Error.Should().BeNull();

        userRepoMock
            .Verify(r => r.AddAsync(It.Is<User>(u => u.Email == newAdmin.email && u.Role == UserRole.Admin),
            It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task GetUserById_WithValidId_ShouldReturnTrue()
    {
        //Arrange
        int userId = 1;

        var userRepoMock = new Mock<IRepository<User>>();

        userRepoMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                Id = userId,
                Name = "Test User",
                Email = "teste@gmail.com",
                Password = "Teste@1234",
                Role = UserRole.User
            });

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);

        //Act
        var result = await _userService.GetUserById(userId, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.id.Should().Be(userId);
    }

    [Fact]
    public async Task GetUserById_WithInvalidId_ShouldReturnFailure()
    {
        //Arrange
        int userId = 99;

        var userRepoMock = new Mock<IRepository<User>>();

        userRepoMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);

        //Act
        var result = await _userService.GetUserById(userId, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.Value.Should().BeNull();
        result.Error.Should().Be("User is not registered");
    }

    [Fact]
    public async Task UpdateUser_WithValidId_ShouldReturnTrue()
    {
        //Arrange
        int userId = 1;

        var updateDto = new UpdateUserDto("Updated Name", "update@email.com", UserRole.Admin);

        var userRepoMock = new Mock<IRepository<User>>();

        userRepoMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                Id = userId,
                Name = "Test User",
                Email = "teste@user.com",
                Password = "Teste@1234",
                Role = UserRole.User
            });

        userRepoMock
            .Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.Users).Returns(userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.CompleteAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        //Act
        var result = await _userService.UpdateUser(userId, updateDto, CancellationToken.None);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Error.Should().BeNull();

        userRepoMock.Verify(
         r => r.UpdateAsync(
             It.Is<User>(u =>
                 u.Id == userId &&
                 u.Name == updateDto.name &&
                 u.Email == updateDto.email &&
                 u.Role == updateDto.role
             ),
             It.IsAny<CancellationToken>()
         )
     );
    }

}