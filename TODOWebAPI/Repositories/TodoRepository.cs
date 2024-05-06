using TODOWebAPI.Data;
using TODOWebAPI.Models;

namespace TODOWebAPI.Repositories
{
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;
        public TodoRepository(TodoDbContext context)
        {
            _context = context;
        }
        public TodoItem AddTodoList(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            _context.SaveChanges();
            return _context.TodoItems.FirstOrDefault(u => u.UserId == todoItem.UserId);
        }

        public void deleteToDoList(TodoItem todoItem)
        {
            _context.TodoItems.Remove(todoItem);
            _context.SaveChanges();
        }

        public TodoItem GetTodoItemByUser(string user,string text)
        {
            return _context.TodoItems.FirstOrDefault(item => item.Text == text && item.UserId == user);
        }

        public List<TodoItem> GetTodoListByUser(string user)
        {
            return _context.TodoItems.Where(item => item.UserId == user).ToList();
        }

        public bool UserExist(string user)
        {
            var userExist = _context.TodoItems.FirstOrDefault(u => u.UserId == user); 
            if(userExist == null)
                return false;
            return true;
        }


    }
}
