using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.WebAPI.Api.Controllers
{
    [Authorize]
    [Route("/api/[controller]")]
    [ApiController]
    public class ListaLeituraController : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public ListaLeituraController(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        private Lista CriarLista(TipoListaLeitura tipo)
        {
            return new Lista
            {
                Tipo = tipo.ParaString(),
                Livros = _repo.All.Where(l => l.Lista == tipo).Select(l => l.ToApi()).ToList()
            };
        }

        [HttpGet]
        public IActionResult TodasListas()
        {
            var listaLer = CriarLista(TipoListaLeitura.ParaLer);
            var listaLendo = CriarLista(TipoListaLeitura.Lendo);
            var listaLido = CriarLista(TipoListaLeitura.Lidos);

            var colecao = new List<Lista> { listaLer, listaLendo, listaLido };
            return Ok(colecao);
        }

        [HttpGet("{tipo}")]
        public IActionResult Recuperar(TipoListaLeitura tipo)
        {
            var lista = CriarLista(tipo);
            return Ok(lista);
        }
    }
}