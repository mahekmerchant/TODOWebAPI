using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TODOWebAPI.Models;
using TODOWebAPI.Repositories;

namespace TODOWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;
        private readonly ILogger<TodoController> _logger;
        public TodoController(ITodoRepository todoRepository, ILogger<TodoController> logger)
        {
            _todoRepository = todoRepository;
            _logger = logger;
        }

        /// <summary>
        /// Gets the list of todo for the given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<TodoItem>> GetTodoList([FromQuery]string user)
        {
            if(string.IsNullOrEmpty(user))
            {
                _logger.LogError("User string cannot be empty or null.");
                return BadRequest("User string cannot be empty or null.");
            }
            bool userExists = _todoRepository.UserExist(user);
            if (!userExists)
            {
                _logger.LogError("User not found, no todo list to delete.");
                return BadRequest("User not found, no todo list to delete.");
            }
            var todoList = _todoRepository.GetTodoListByUser(user);
            return Ok(todoList);
        }

        /// <summary>
        /// Adds the todo item to the todo list for the given user
        /// </summary>
        /// <param name="todoParameters"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<TodoItem> AddTodoItem([FromBody]TodoParameters todoParameters)
        {
            if (todoParameters==null)
            {
                _logger.LogError("body parameters cannot be null.");
                return BadRequest("body parameters cannot be null.");
            }
            if (string.IsNullOrEmpty(todoParameters.UserId) || string.IsNullOrEmpty(todoParameters.Text))
            {
                _logger.LogError("User or text string cannot be empty or null.");
                return BadRequest("User or text string cannot be empty or null.");
            }
            var todoItem = new TodoItem
            {
                UserId = todoParameters.UserId,
                Text = todoParameters.Text
            };

            var todoItemResp = _todoRepository.AddTodoList(todoItem);

            return Ok(todoItemResp);
        }

        /// <summary>
        /// Deletes the todo item from the todo list for the given user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public IActionResult DeleteTodoItem([FromQuery]string user, [FromQuery]string text)
        {
            bool userExists=_todoRepository.UserExist(user);
            if (!userExists)
            {
                _logger.LogError("User not found, no todo list to delete.");
                return BadRequest("User not found, no todo list to delete.");
            }
            var todoItem = _todoRepository.GetTodoItemByUser(user,text);
            if (todoItem == null)
            {
                _logger.LogError("TODO item not found or does not belong to the user.");
                return BadRequest("TODO item not found or does not belong to the user.");
            }

            _todoRepository.deleteToDoList(todoItem);

            return NoContent();
        }


    }
}
