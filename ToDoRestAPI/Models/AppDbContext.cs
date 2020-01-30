using Microsoft.EntityFrameworkCore;

namespace ToDoRestAPI.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Todo> Todo {get; set;}
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }
    }
}