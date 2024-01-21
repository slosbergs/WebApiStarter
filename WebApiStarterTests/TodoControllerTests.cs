using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WebApiStarter.Controllers;
using WebApiStarter.Infrastructure;
using WebApiStarterTests.Mocks;
using Xunit;

namespace WebApiStarterTests;



public class TodoControllerTests
{
    private readonly Mock<ILogger<TodoController>> _mockLogger;

    public TodoControllerTests()
    {
        _mockLogger = new Mock<ILogger<TodoController>>();
    }

    [Fact]
    public async Task GetAsync_ValidUserId_ReturnsTodos()
    {
        // Arrange
        int userId = 1; // Example user ID for testing
        var mockTodoClient = new TodoSystemApiClientMock().GetByUserIdAsync(TodoMocks.Array);
        TodoController _controller = new TodoController(_mockLogger.Object, mockTodoClient.Object);

        // Act
        var result = await _controller.GetAsync(userId);

        // Assert
        Assert.NotEmpty(result);
        Assert.True(TodoMocks.Array.Count() == result.Count());
    }

    [Fact]
    public async Task GetAsync_UserId_ReturnsEmptyListWhenNoTodos()
    {
        // Arrange
        int userId = 1; // Example user ID where no todos are expected
        var mockTodoClient = new TodoSystemApiClientMock().GetByUserIdAsync(TodoMocks.EmptyArray);
        TodoController _controller = new TodoController(_mockLogger.Object, mockTodoClient.Object);


        // Act
        var result = await _controller.GetAsync(userId);

        // Assert
        Assert.Empty(result);
    }

    // Add more test cases as necessary
}