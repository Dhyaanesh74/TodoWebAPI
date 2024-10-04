using System.Formats.Asn1;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using todowebapi.DataAnnotations;
using todowebapi.Models;
using todowebapi.Exceptions;
using CustomKeyNotFoundException = todowebapi.Exceptions.KeyNotFoundException;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace todowebapi.Controllers
{
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    [Microsoft.AspNetCore.Mvc.Route("api/v1/[controller]")]
    [ApiController]

    public class TodoController : ControllerBase
    {
         private readonly TodoDbContext _context;

         public TodoController(TodoDbContext context) => _context = context;

         [HttpGet]
         public async Task<IActionResult> get(){
            var TodoList  =  await _context.Todo.ToListAsync();
            return Ok(TodoList);
         }
          
         [HttpGet("{Id}")]

         public async Task<IActionResult> getbyId(int id){
            var res =  await _context.Todo.FindAsync(id);
            if (res == null)
            {
                throw new BadRequestException("Todo item not found.");
            }
            return Ok(res);
         }
         [HttpGet("search/{Title}")]
         public async Task<IActionResult> FindByTitle(string Title){
            var res = await _context.Todo.SingleOrDefaultAsync(t => t.Title == Title);
            if(res == null) throw new NotFoundException("Title not found");
            return Ok(res);
            
         }

         [HttpGet("SearchByAutor/{Author}")]
         public async Task<IActionResult> SearchByAuthor(string Author){
            var res =  await _context.Todo.Where(t => t.Author == Author).ToListAsync();
            if(res == null) throw new NotFoundException("Title not found");
            return Ok(res);
        }
        [HttpGet("Search")]
        public async Task<IActionResult> getByQueryParams([FromQuery] string? Author, [FromQuery] int? type){
            var query =  _context.Todo.AsQueryable();

            if(!string.IsNullOrEmpty(Author)) query = query.Where(t => t.Author == Author);
            if (type != null)
            {
                query = query.Where(t => (int)t.type == type);  
            }  
            var res =  await query.ToListAsync();
            if(res.Count == 0) return NotFound("No results found");
            return Ok(res);
        }
         
        [HttpPost]
        public async Task<IActionResult> Create(Todo todo)
        {

            if(todo.Id == null) throw new CustomKeyNotFoundException("Id is required");
            await _context.Todo.AddAsync(todo);
            await _context.SaveChangesAsync();
            return Ok("Created Sucessfully");
        }

    [HttpPost("List")]
    public async Task<IActionResult> Create(List<Todo> todos)
    {

        await _context.Todo.AddRangeAsync(todos);
        await _context.SaveChangesAsync();
        return Ok("Created Successfully");
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int Id, Todo todo){
        if(Id != todo.Id) return BadRequest();
        _context.Entry(todo).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Ok("Updated");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await _context.Todo.FindAsync(id);  
        if(res==null)
        {
            return NotFound(); 
        }
        _context.Todo.Remove(res);  
        await _context.SaveChangesAsync();  
        return Ok("Deleted");  
    }




    }
}