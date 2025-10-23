using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Fcg.Domain;
using Fcg.Application.Services;
using Fcg.Data.Repositories.Interface;
using Castle.Core.Configuration;
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
    }
}