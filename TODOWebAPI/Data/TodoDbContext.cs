using Microsoft.EntityFrameworkCore;
using TODOWebAPI.Models;

namespace TODOWebAPI.Data
{
    public class TodoDbContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }

        public TodoDbContext(DbContextOptions<TodoDbContext> options)
       : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("TodoListDb"); 
        }
    }
}
