using TODOWebAPI.Models;

namespace TODOWebAPI.Repositories
{
    public interface ITodoRepository
    {
        List<TodoItem> GetTodoListByUser(string user);
        TodoItem GetTodoItemByUser(string user,string text);
        TodoItem AddTodoList(TodoItem todoItem);

        void deleteToDoList(TodoItem todoItem);

        bool UserExist(string user);
    }
}
