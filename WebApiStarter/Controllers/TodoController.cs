using WebApiStarter.Infrastructure;
using WebApiStarter.Model;
using Microsoft.AspNetCore.Mvc;

namespace WebApiStarter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ILogger<TodoController> _logger;
        private readonly TodoSystemApiClient _todoClient;

        public TodoController(ILogger<TodoController> logger, TodoSystemApiClient todoClient)
        {
            _logger = logger;
            _todoClient = todoClient;
        }

        [HttpGet("{userId}", Name = "GetTodo")]
        public async Task<IEnumerable<Todo>> GetAsync(int userId)
        {
            return await _todoClient.GetUserTodosAsync(userId);
        }

    }
}
