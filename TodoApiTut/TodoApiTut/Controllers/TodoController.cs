using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApiTut.Models;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApiTut.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;

            if (_context.TodoItems.Count() == 0)
            {
                // Create a new TodoItem if collection is empty,
                // which means you can't delete all TodoItems.
                _context.TodoItems.Add(new TodoItem { Name = "Item1" });
                _context.SaveChanges();
            }
        }

        // GET: api/Todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/Todo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // POST: api/Todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem item)
        {
            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoItem), new { id = item.Id }, item);
        }

        // PUT: api/Todo/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem item)
        {
            // If 'id' (incoming id param) does not equal the 'item' in the DB, 
              // return a BadRequest/400 status result
            if (id != item.Id)
            {
                return BadRequest();
            }

            // Gets an EntityEntry for the given entity. The entry provides access to change tracking
              // info and operations for the entity.
              // The state in which an entity is being tracked by a context
              // The entity is being tracked by the context and exists in the DB. Some or all of
                // its property values have been modified
            _context.Entry(item).State = EntityState.Modified;
            // Asynchronously saves all changes made in this context to the DB
            await _context.SaveChangesAsync();

            // Creates a 'NoContentResult' object that produces an empty 204 No Content Status response
            return NoContent();
        }

        // DELETE: api/Todo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            // Create 'todoItem' variable, which will hold the entity with the given id
            var todoItem = await _context.TodoItems.FindAsync(id);

            // If 'todoItem' is null, return a NotFound/404 status result
            if (todoItem == null)
            {
                return NotFound();
            }

            // Begins tracking the given entity in the EntityState.Deleted state such that 
              // it will be removed from the DB when DB.SaveChanges() is called.
            _context.TodoItems.Remove(todoItem);
            // Asynchronously saves all changes made in this context to the DB
            await _context.SaveChangesAsync();

            // Creates a 'NoContentResult' object that produces an empty 204 No Content Status response
            return NoContent();
        }
    }
}
