using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using todowebapi.Models;

namespace todowebapi.DataAnnotations{
    public class TodoDbContext : IdentityDbContext{
        public TodoDbContext(DbContextOptions<TodoDbContext>options) : base(options){
              
        }

        public DbSet<Todo> Todo { get; set;}
    }
}
