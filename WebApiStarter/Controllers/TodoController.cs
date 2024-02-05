using WebApiStarter.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using WebApiStarter.Domain.Model;

namespace WebApiStarter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ILogger<TodoController> _logger;
        private readonly ITodoSystemApiClient _todoClient;

        public TodoController(ILogger<TodoController> logger, ITodoSystemApiClient todoClient)
        {
            _logger = logger;
            _todoClient = todoClient;
        }

        [HttpGet("{userId}", Name = "GetTodo")]
        public async Task<IEnumerable<Todo>> GetAsync(int userId)
        {
            return await _todoClient.GetUserTodosAsync(userId);
        }

        [HttpPost("{userId}", Name = "CreateTodo")]
        public async Task<int> PostAsync(int userId, [FromBody] Todo item)
        {
            var newId = await _todoClient.CreateAsync(userId, item);
            return newId;
        }

    }
}
