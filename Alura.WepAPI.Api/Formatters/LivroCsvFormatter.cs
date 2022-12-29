using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Alura.WebAPI.Api.Formatters
{
    public class LivroCsvFormatter : TextOutputFormatter
    {
        public LivroCsvFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/csv"));
            SupportedEncodings.Add(Encoding.UTF8);
        }

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            var acceptHeader = context.HttpContext.Request.Headers["Accept"];

             return (context.Object is LivroApi || context.Object is ICollection<LivroApi>) && ((new List<string> { "text/csv", "application/csv" }).Contains(acceptHeader));
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var outputCSV = "";

            if (context.Object is LivroApi)
            {
                var livro = context.Object as LivroApi;
                outputCSV = LivroToCsv(livro);
            }
            else if (context.Object is ICollection<LivroApi>)
            {
                var livros = context.Object as List<LivroApi>;
                var sb = new StringBuilder();
                livros.ForEach(l => sb.AppendLine(LivroToCsv(l)));
                outputCSV = sb.ToString();
            }

            using (var escritor = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            {
                return escritor.WriteAsync(outputCSV);
            } //escritor.Close();
        }

        private static string LivroToCsv(LivroApi livro)
        {
            return $"{livro.Titulo};{livro.Subtitulo};{livro.Autor};{livro.Lista}";
        }
    }
}