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

    }
}
