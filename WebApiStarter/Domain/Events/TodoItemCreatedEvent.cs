using WebApiStarter.Domain.Model;
namespace WebApiStarter.Domain.Events;

public class TodoItemCreatedNotification
{
    public Todo Item { get; set; }
}
