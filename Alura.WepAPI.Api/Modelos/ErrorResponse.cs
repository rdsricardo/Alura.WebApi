using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.WepAPI.Api.Modelos
{
    public class ErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public ErrorResponse InnerError { get; set; }
        public string[] Detalhes { get; set; }

        public static ErrorResponse From(Exception e)
        {
            if (e == null)
                return null;

            return new ErrorResponse
            {
                Code = e.HResult,
                Message = e.Message,
                InnerError = ErrorResponse.From(e.InnerException)
            };
        }

        public static ErrorResponse FromModelState(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(m => m.Errors);

            return new ErrorResponse
            {
                Code = 99,
                Message = "Houve erro(s) no envio da requisição",
                InnerError = null,
                Detalhes = erros.Select(e => e.ErrorMessage).ToArray()
            };
        }
    }


}
