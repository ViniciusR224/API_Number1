using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Net;

namespace API_Number1.Filters
{
    public class ExceptionFilterController :ExceptionFilterAttribute
    {
        public async override Task OnExceptionAsync(ExceptionContext context)//No caso o context(httpcontext) onde a exception surgiu      -
        {                                                              //e mais informações como a própria exception e outras coisas -
                                                                       //Exception, Result, HttpContext, ActionDescriptor, entre outras.
            
            var ex =context.Exception;
            
            var problemDetails=new ProblemDetails()
            {
                Title = "Internal Server Error CT",
                Status=(int)HttpStatusCode.InternalServerError,
                Detail=$"{ex.Message}+{ex.StackTrace}",
                Instance=ex.Source,
                Type=ex.InnerException?.Message,                
                
            };
            var json=JsonConvert.SerializeObject(problemDetails);
            var content = new ContentResult
            {               
                Content = json,
                ContentType = "application/problem+json",
                StatusCode = (int)HttpStatusCode.InternalServerError,
            };
            context.ExceptionHandled = true;
            
            
            await content.ExecuteResultAsync(new ActionContext { HttpContext = context.HttpContext });

            
        }
    }
}
