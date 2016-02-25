using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using System.Security.Claims;
using TodoListService.Models;
using System.Collections.Concurrent;

namespace TodoListService.Controllers
{
    using Microsoft.AspNet.Authorization;

    [Authorize]
    [Route("api/[controller]")]
    public class TodoListController : Controller
    {
        static ConcurrentBag<TodoItem> todoStore = new ConcurrentBag<TodoItem>();

        // GET: api/todolist
        [HttpGet]
        public IEnumerable<TodoItem> Get()
        {
            // Please note: use of "Context.User", instead of the standard ClaimsPrincipal.Current, is due to a bug in this release
            string owner = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return todoStore.Where(t => t.Owner == owner).ToList();
        }
    
        // POST api/todolist
        [HttpPost]
        public void Post(string Title)
        {
            // Please note: use of "Context.User", instead of the standard ClaimsPrincipal.Current, is due to a bug in this release
            string owner = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            todoStore.Add(new TodoItem { Owner = owner, Title = Title });
        }
    }
}
