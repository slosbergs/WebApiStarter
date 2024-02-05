using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiStarter.Domain.Model;

namespace WebApiStarterTests.Mocks
{
    internal class TodoMocks
    {
        public static Todo[] Array => new[] { new Todo(), new Todo() };

        public static Todo[] EmptyArray => new Todo[] { };
    }
}
