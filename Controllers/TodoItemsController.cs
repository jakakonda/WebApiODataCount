using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    #region TodoController
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
            var category = new Category() { Name = "Test" };
            _context.Categories.Add(category);
            _context.SaveChanges();

            _context.TodoItems.Add(new TodoItem
            {
                Name = "1",
                IsComplete = true,
                // Category = category,
            });
            _context.TodoItems.Add(new TodoItem
            {
                Name = "2",
                IsComplete = true,
                // Category = category,
            });
            _context.TodoItems.Add(new TodoItem
            {
                Name = "3",
                IsComplete = true,
                // Category = category,
            });
            _context.SaveChanges();
        }
        #endregion

        public class Result<T>
        {
            public int Count { get; set; }
            public IEnumerable<T> Value { get; set; }

            public Result(IEnumerable<T> value, int count)
            {
                Value = value;
                Count = count;
            }
        }

        // GET: api/TodoItems
        // With paging
        // [HttpGet]
        // public Result<TodoItem> GetTodoItems(ODataQueryOptions<TodoItem> options)
        // {
        //     var settings = new ODataQuerySettings()
        //     {
        //         PageSize = 5
        //     };
        //
        //     IQueryable results = options.ApplyTo(_context.TodoItems, settings);
        //
        //     return new Result<TodoItem>(results as IEnumerable<TodoItem>, 5);
        // }

        [HttpGet]
        public IEnumerable<TodoItem> GetTodoItems(ODataQueryOptions<TodoItem> options)
        {
            var settings = new ODataQuerySettings()
            {
                PageSize = 5,
            };

            var results = options.ApplyTo(_context.TodoItems.AsQueryable(), settings);

            return new PageResult<TodoItem>(
                results as IQueryable<TodoItem>,
                new Uri("https://irrelevant"),
                5);
        }

        // GET: api/TodoItems/5
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

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        #region snippet_Update
        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        #endregion

        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        #region snippet_Create
        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }
        #endregion

        #region snippet_Delete
        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        #endregion

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
