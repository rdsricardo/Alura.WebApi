using Alura.ListaLeitura.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alura.WepAPI.Api.Modelos
{
    public static class LivroPaginadoExtensions
    {
        public static LivroPaginado ToLivroPaginado(this IQueryable<LivroApi> query, LivroPaginacao paginacao)
        {
            int totalItens = query.Count();
            int totalPaginas = (int)Math.Ceiling(totalItens / (double)paginacao.Tamanho);
            return new LivroPaginado
            {
                Total = query.Count(),
                TotalPaginas = totalPaginas,
                TamanhoPagina = paginacao.Tamanho,
                NumeroPagina = paginacao.Pagina,
                Resultado = query
                    .Skip(paginacao.Tamanho * (paginacao.Pagina - 1))
                    .Take(paginacao.Tamanho)
                    .ToList(),
                Anterior = (paginacao.Pagina > 1) ? $"/Livros?pagina={ paginacao.Pagina - 1 }&tamanho={ paginacao.Tamanho }" : "",
                Proximo = (paginacao.Pagina < totalPaginas) ? $"/Livros?pagina={ paginacao.Pagina + 1 }&tamanho={ paginacao.Tamanho }" : "",
            };
        }
    }

    public class LivroPaginado
    {
        public int Total { get; set; }
        public int TotalPaginas { get; set; }
        public int TamanhoPagina { get; set; }
        public int NumeroPagina { get; set; }
        public IList<LivroApi> Resultado { get; set; }
        public string Anterior { get; set; }
        public string Proximo { get; set; }
    }

    public class LivroPaginacao
    {
        public int Pagina { get; set; } = 1;
        public int Tamanho { get; set; } = 25;
    }
}