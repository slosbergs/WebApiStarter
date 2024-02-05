using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiStarter.Domain.Model;
using WebApiStarter.Infrastructure;

namespace WebApiStarterTests.Mocks;



public class TodoSystemApiClientMock : Mock<ITodoSystemApiClient>
{
    public TodoSystemApiClientMock GetByUserIdAsync(Todo[] returnValue)
    {
        Setup(x => x.GetUserTodosAsync(It.IsAny<int>()))
            .Returns(Task.FromResult(returnValue));
                    //.ReturnsAsync<IEnumerable<Todo>>(returnValue);

        return this;
    }
}