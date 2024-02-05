using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Elastic.CommonSchema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace WebApiStarter.Domain
{

    public class ExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is Exception unhandledException)
            {
                var problemDetails = new ProblemDetails
                {
                    Detail = unhandledException.Message,
                    Instance = context.HttpContext.Request.GetDisplayUrl(),
                    Status = 500,
                    Title = "Server Error",
                    Type = unhandledException.GetType().FullName,
                };
                context.Result = new ObjectResult(problemDetails)
                {
                    StatusCode = problemDetails.Status
                };

                context.ExceptionHandled = true;
            }
        }
    }
}
