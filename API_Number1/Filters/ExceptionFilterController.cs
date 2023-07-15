using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Xml.Serialization;
using static System.Net.WebRequestMethods;

namespace API_Number1.Filters
{
    public class ExceptionFilterController : ExceptionFilterAttribute
    {
        public async override Task OnExceptionAsync(ExceptionContext context)//No caso o context(httpcontext) onde a exception surgiu      -
        {                                                              //e mais informações como a própria exception e outras coisas -
                                                                       //Exception, Result, HttpContext, ActionDescriptor, entre outras.
            var typesAccpeted = context.HttpContext.Request.Headers.Accept;
            var ex = context.Exception;

            var problemDetails = CreateProblemDetails(ex);

            //var contentTyped = CreateContentResult(problemDetails);
            var objectResult=CreateObjectResult(problemDetails);


            context.ExceptionHandled = true;
            //context.Result = contentTyped;
            context.Result = objectResult;
        }

        //Por esses métodos serem somente necessários nessa classe é melhor deixar private


        private ProblemDetails CreateProblemDetails(Exception ex)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Internal Server Error CT",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = $"{ex.Message}+{ex.StackTrace}",
                Instance = ex.Source,
                Type = "https://httpstatuses.io/500"
            };

            return problemDetails;
        }
        
        //Util para retorno de objetos json ou qualquer tipo, você precisa fazer as configurações corretas nos formatadores de conteudo
        //para conseguir retornar o tipo especifico que esta no Header Accept, sem eles, por padrão será retornado em json.
        //Mesmo que lá esteja xml por exemplo - do caralho essa porra hue
        private ObjectResult CreateObjectResult(ProblemDetails problemDetails)
        {
            
            var objectResult = new ObjectResult(problemDetails)
            {
                StatusCode = (int)HttpStatusCode.BadRequest,               
            };
            return objectResult;
        }


        //Mais utilizado para retornos simples como mensagens e coisas do tipo,  
        //Mas é necessário serializer no tipo que está no accept e que quer retornar 
        private ContentResult CreateContentResult(ProblemDetails problemDetails)
        {
            var json = JsonConvert.SerializeObject(problemDetails).ToString();           
            var content = new ContentResult
            {
                Content = json, 
                ContentType = "text/plain",
                StatusCode = (int)HttpStatusCode.InternalServerError,

            };

            return content;
        }
    }
}
