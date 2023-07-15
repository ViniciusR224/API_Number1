using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;
using System.Text.Json;

namespace API_Number1.Middlewares
{
    public class ExceptionMiddleware : IMiddleware
    {
        
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
			try
			{
				await next(context);
			}
			catch (Exception e)//Diferente do Handler, aqui tenho acesso direto a exception, não preciso invocar o IExceptionHandlerFeature
            {
				
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
				context.Response.Headers.ContentType = "application/json";
				ProblemDetails details = new()
				{
					
					Status = (int)HttpStatusCode.InternalServerError,
					Title = e.Message,
					Instance = e.InnerException?.Message,
					Detail = "Mensagem Padrão de Exception GL",
					Type = "https://httpstatuses.io/500",
				};
				var json = JsonConvert.SerializeObject(details);
				await context.Response.WriteAsync(json);
				
			}
        }
    }
}
