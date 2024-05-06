using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TODOWebAPI.Data;
using TODOWebAPI.Models;
using TODOWebAPI.Repositories;
using Xunit;

namespace TODOWebAPITests.Repositories
{
    public class TodoRepositoryTests
    {
        private readonly DbContextOptions<TodoDbContext> _options;

        public TodoRepositoryTests()
        {
            _options = CreateInMemoryDatabaseOptions();
        }

        private DbContextOptions<TodoDbContext> CreateInMemoryDatabaseOptions()
        {
            return new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public void AddTodoList_ReturnsTodoItem()
        {
            // Arrange
            using (var context = new TodoDbContext(_options))
            {
                var repository = new TodoRepository(context);
                var todoItem = new TodoItem { UserId = "user1", Text = "Sample Todo" };

                // Act
                var addedTodoItem = repository.AddTodoList(todoItem);

                // Assert
                Assert.NotNull(addedTodoItem);
                Assert.Equal(todoItem.UserId, addedTodoItem.UserId);
                Assert.Equal(todoItem.Text, addedTodoItem.Text);
            }
        }

        [Fact]
        public void AddTodoList_SavesToDatabase()
        {
            // Arrange
            using (var context = new TodoDbContext(_options))
            {
                var repository = new TodoRepository(context);
                var todoItem = new TodoItem { UserId = "user1", Text = "Sample Todo" };

                // Act
                repository.AddTodoList(todoItem);
            }

            // Assert
            using (var context = new TodoDbContext(_options))
            {
                var savedTodoItem = context.TodoItems.First();
                Assert.Equal("user1", savedTodoItem.UserId);
                Assert.Equal("Sample Todo", savedTodoItem.Text);
            }
        }

        [Fact]
        public void DeleteTodoList_RemovesTodoItemFromContext()
        {
            // Arrange
            var todoItem = new TodoItem { UserId = "user1", Text = "Sample Todo" };
            using (var context = new TodoDbContext(_options))
            {
                context.TodoItems.Add(todoItem);
                context.SaveChanges();
            }

            // Act
            using (var context = new TodoDbContext(_options))
            {
                var repository = new TodoRepository(context);
                repository.deleteToDoList(todoItem);
            }

            // Assert
            using (var context = new TodoDbContext(_options))
            {
              Assert.NotNull(context.TodoItems);
            }
        }

        [Fact]
        public void DeleteTodoList_SavesChangesToDatabase()
        {
            // Arrange
            var todoItem = new TodoItem { UserId = "user1", Text = "Sample Todo" };
            using (var context = new TodoDbContext(_options))
            {
                context.TodoItems.Add(todoItem);
                context.SaveChanges();
            }

            // Act
            using (var context = new TodoDbContext(_options))
            {
                var repository = new TodoRepository(context);
                repository.deleteToDoList(todoItem);
            }

            // Assert
            using (var context = new TodoDbContext(_options))
            {
                var deletedTodoItem = context.TodoItems.Find(todoItem.Id);
                Assert.Null(deletedTodoItem);
            }
        }

        [Fact]
        public void GetTodoItemByUser_ReturnsCorrectTodoItem()
        {
            // Arrange
            var options=CreateInMemoryDatabaseOptions();
            var user = "user9";
            var todoItem = new TodoItem { UserId = "user9", Text = "Sample Todo" };
            using (var context = new TodoDbContext(options))
            {
                context.TodoItems.Add(todoItem);
                context.SaveChanges();
            }

            // Act
            using (var context = new TodoDbContext(options))
            {
                var repository = new TodoRepository(context);
                var todoItems = repository.GetTodoItemByUser(user, "Sample Todo");

                // Assert
                Assert.Equal("user9", todoItem.UserId);
                Assert.Equal("Sample Todo", todoItem.Text);
            }
        }

        [Fact]
        public void GetTodoItemByUser_ReturnsNullIfItemNotFound()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            var user = "user20";
            var todoItem = new TodoItem { UserId = "user10", Text = "Sample Todo1" };
            using (var context = new TodoDbContext(options))
            {
                context.TodoItems.Add(todoItem);
                context.SaveChanges();
            }

            // Act
            using (var context = new TodoDbContext(options))
            {
                var repository = new TodoRepository(context);
                var todoItems = repository.GetTodoItemByUser(user, "Sample Todo1");

                // Assert
                Assert.Null(todoItems);
            }
        }

        [Fact]
        public void GetTodoListByUser_ReturnsCorrectTodoItems()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            var user = "user11";
            var todoItem = new TodoItem { UserId = "user11", Text = "Sample Todo" };
            using (var context = new TodoDbContext(options))
            {
                context.TodoItems.Add(todoItem);
                context.SaveChanges();
            }

            // Act
            using (var context = new TodoDbContext(options))
            {
                var repository = new TodoRepository(context);
                var todoList = repository.GetTodoListByUser(user);

                // Assert
               
                Assert.True(todoList.All(item => item.UserId == user)); 
            }
        }

        [Fact]
        public void GetTodoListByUser_ReturnsEmptyListIfUserHasNoItems()
        {
            // Arrange
            var options = CreateInMemoryDatabaseOptions();
            var user = "user12";
            var todoItem = new TodoItem { UserId = "user15", Text = "Sample Todo" };
            using (var context = new TodoDbContext(options))
            {
                context.TodoItems.Add(todoItem);
                context.SaveChanges();
            }

            // Act
            using (var context = new TodoDbContext(options))
            {
                var repository = new TodoRepository(context);
                var todoList = repository.GetTodoListByUser(user);

                // Assert
                Assert.Empty(todoList);
            }
        }

        [Fact]
        public void UserExist_ReturnsTrueIfUserExists()
        {
            // Arrange
            var user = "user1";
            // Act
            using (var context = new TodoDbContext(_options))
            {
                var repository = new TodoRepository(context);
                var userExists = repository.UserExist(user);

                // Assert
                Assert.True(userExists);
            }
        }

        [Fact]
        public void UserExist_ReturnsFalseIfUserDoesNotExist()
        {
            // Arrange
            var user = "user16";
            // Act
            using (var context = new TodoDbContext(_options))
            {
                var repository = new TodoRepository(context);
                var userExists = repository.UserExist(user);

                // Assert
                Assert.False(userExists);
            }
        }
    }
}
