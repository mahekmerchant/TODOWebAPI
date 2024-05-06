using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TODOWebAPI.Controllers;
using TODOWebAPI.Models;
using TODOWebAPI.Repositories;
using Xunit;

namespace TODOWebAPITests.Controllers
{
    public class TodoControllerTests
    {
        [Fact]
        public void GetTodoList_WithNullOrEmptyUser_ReturnsBadRequest()
        {
            // Arrange
            var mockTodoRepository = new Mock<ITodoRepository>();
            var mockLogger = new Mock<ILogger<TodoController>>();
            var controller = new TodoController(mockTodoRepository.Object, mockLogger.Object);

            // Act
            var result = controller.GetTodoList(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User string cannot be empty or null.", badRequestResult.Value);
        }

        [Fact]
        public void GetTodoList_WithNonExistingUser_ReturnsBadRequest()
        {
            // Arrange
            var mockTodoRepository = new Mock<ITodoRepository>();
            mockTodoRepository.Setup(repo => repo.UserExist(It.IsAny<string>())).Returns(false);
            var mockLogger = new Mock<ILogger<TodoController>>();
            var controller = new TodoController(mockTodoRepository.Object, mockLogger.Object);

            // Act
            var result = controller.GetTodoList("nonExistingUser");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User not found, no todo list to delete.", badRequestResult.Value);
        }

        [Fact]
        public void GetTodoList_WithExistingUser_ReturnsOkWithTodoList()
        {
            // Arrange
            var user = "existingUser";
            var todoItems = new List<TodoItem>
            {
                new TodoItem { Id = 1, UserId = user, Text = "Todo 1" },
                new TodoItem { Id = 2, UserId = user, Text = "Todo 2" }
            };
            var mockTodoRepository = new Mock<ITodoRepository>();
            mockTodoRepository.Setup(repo => repo.UserExist(user)).Returns(true);
            mockTodoRepository.Setup(repo => repo.GetTodoListByUser(user)).Returns(todoItems);
            var mockLogger = new Mock<ILogger<TodoController>>();
            var controller = new TodoController(mockTodoRepository.Object, mockLogger.Object);

            // Act
            var result = controller.GetTodoList(user);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<List<TodoItem>>(okResult.Value);
            Assert.Equal(todoItems.Count, model.Count);
        }

        [Fact]
        public void AddTodoItem_WithNullTodoParameters_ReturnsBadRequest()
        {
            // Arrange
            var mockTodoRepository = new Mock<ITodoRepository>();
            var mockLogger = new Mock<ILogger<TodoController>>();
            var controller = new TodoController(mockTodoRepository.Object, mockLogger.Object);

            // Act
            var result = controller.AddTodoItem(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("body parameters cannot be null.", badRequestResult.Value);
        }

        [Theory]
        [InlineData(null, "Todo text")]
        [InlineData("User", null)]
        [InlineData(null, null)]
        [InlineData("", "Todo text")]
        [InlineData("User", "")]
        [InlineData("", "")]
        public void AddTodoItem_WithNullOrEmptyUserIdOrText_ReturnsBadRequest(string userId, string text)
        {
            // Arrange
            var todoParameters = new TodoParameters { UserId = userId, Text = text };
            var mockTodoRepository = new Mock<ITodoRepository>();
            var mockLogger = new Mock<ILogger<TodoController>>();
            var controller = new TodoController(mockTodoRepository.Object, mockLogger.Object);

            // Act
            var result = controller.AddTodoItem(todoParameters);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User or text string cannot be empty or null.", badRequestResult.Value);
        }

        [Fact]
        public void AddTodoItem_WithValidTodoParameters_ReturnsOkWithTodoItem()
        {
            // Arrange
            var todoParameters = new TodoParameters { UserId = "userId", Text = "Todo text" };
            var todoItem = new TodoItem { UserId = "userId", Text = "Todo text" };
            var mockTodoRepository = new Mock<ITodoRepository>();
            mockTodoRepository.Setup(repo => repo.AddTodoList(todoItem)).Returns(todoItem);
            var mockLogger = new Mock<ILogger<TodoController>>();
            var controller = new TodoController(mockTodoRepository.Object, mockLogger.Object);

            // Act
            var result = controller.AddTodoItem(todoParameters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void DeleteTodoItem_WithNonExistingUser_ReturnsBadRequest()
        {
            // Arrange
            var mockTodoRepository = new Mock<ITodoRepository>();
            mockTodoRepository.Setup(repo => repo.UserExist(It.IsAny<string>())).Returns(false);
            var mockLogger = new Mock<ILogger<TodoController>>();
            var controller = new TodoController(mockTodoRepository.Object, mockLogger.Object);

            // Act
            var result = controller.DeleteTodoItem("nonExistingUser", "list1");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User not found, no todo list to delete.", badRequestResult.Value);
        }

        [Fact]
        public void DeleteTodoItem_WithNonExistingTodoItem_ReturnsBadRequest()
        {
            // Arrange
            var user = "existingUser";
            var mockTodoRepository = new Mock<ITodoRepository>();
            mockTodoRepository.Setup(repo => repo.UserExist(user)).Returns(true);
            mockTodoRepository.Setup(repo => repo.GetTodoItemByUser(user, "list1")).Returns<TodoItem>(null);
            var mockLogger = new Mock<ILogger<TodoController>>();
            var controller = new TodoController(mockTodoRepository.Object, mockLogger.Object);

            // Act
            var result = controller.DeleteTodoItem(user, "list1");

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("TODO item not found or does not belong to the user.", badRequestResult.Value);
        }

        [Fact]
        public void DeleteTodoItem_WithValidUserAndTodoItem_ReturnsNoContent()
        {
            // Arrange
            var user = "existingUser";
            var todoItem = new TodoItem { Id = 1, UserId = user, Text = "Todo text" };
            var mockTodoRepository = new Mock<ITodoRepository>();
            mockTodoRepository.Setup(repo => repo.UserExist(user)).Returns(true);
            mockTodoRepository.Setup(repo => repo.GetTodoItemByUser(user, "list1")).Returns(todoItem);
            var mockLogger = new Mock<ILogger<TodoController>>();
            var controller = new TodoController(mockTodoRepository.Object, mockLogger.Object);

            // Act
            var result = controller.DeleteTodoItem(user, "list1");

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockTodoRepository.Verify(repo => repo.deleteToDoList(todoItem), Times.Once);
        }
    }
}
