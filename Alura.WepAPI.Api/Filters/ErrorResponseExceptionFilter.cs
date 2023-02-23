using Alura.WepAPI.Api.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Alura.WepAPI.Api.Filters
{
    public class ErrorResponseExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new ObjectResult(ErrorResponse.From(context.Exception)) { StatusCode = 500 };
        }
    }
}