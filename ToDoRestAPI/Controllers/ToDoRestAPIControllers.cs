using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ToDoRestAPI.Models;

namespace ToDoRestAPI.Controllers
{
    [Route("/todo")]
    [ApiController]
    public class ToDoRestAPIControllers
    {
        private AppDbContext _appDbContext;

        public ToDoRestAPIControllers(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Todo>> Get()
        {
            return _appDbContext.Todo;
        }
        [HttpDelete("{id}")]
        public ActionResult<string> Delete(int id)
        {
            var Delete = _appDbContext.Todo.Find(id);
            _appDbContext.Attach(Delete);
            _appDbContext.Remove(Delete);
            _appDbContext.SaveChanges();

            return $"Menghapus data ID: {id}";
        }
        [HttpPatch("{id}")]
        public ActionResult<Todo> Update(int id, [FromBody] Todo todo)
        {
            var todo1 = _appDbContext.Todo.Find(id);
            todo1.kegiatan = todo.kegiatan;
            _appDbContext.SaveChanges();

            return todo1;
        }
        
        [HttpPost]
        public ActionResult<Todo> Post([FromBody] Todo todo)
        {
            _appDbContext.Add(todo);
            _appDbContext.SaveChanges();

            return todo;
        }
        
        [HttpPatch]
        [Route("/todo/done/{id}")]
        public ActionResult<Todo> Up(bool selesai, [FromBody] Todo todo)
        {
            var todo1 = _appDbContext.Todo.Find(selesai);
            todo1.selesai = todo.selesai;
            _appDbContext.SaveChanges();

            return todo1;
        }
    }
}