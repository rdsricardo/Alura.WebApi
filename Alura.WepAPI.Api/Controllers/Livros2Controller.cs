using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Persistencia;
using Alura.WepAPI.Api.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;

namespace Alura.WepAPI.Api.Controllers
{
    [Authorize]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/v{version:apiVersion}/Livros")]
    [ApiController]
    public class Livros2Controller : ControllerBase
    {
        private readonly IRepository<Livro> _repo;

        public Livros2Controller(IRepository<Livro> repository)
        {
            _repo = repository;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Recupera todos os livros paginados e com opção de filtro e ordem.",
            Tags = new string[] { "Livros" }
        )]
        [Produces("application/json")]
        [ProducesResponseType(statusCode: 200, Type = typeof(LivroPaginado))]
        [ProducesResponseType(statusCode: 500, Type = typeof(ErrorResponse))]
        //[ProducesResponseType(statusCode: 401)]
        public IActionResult RecuperarTodos([FromQuery] [SwaggerParameter("Opções de filtragem")] LivroFiltro filtro,
            [FromQuery] [SwaggerParameter("Opções de ordenação")] LivroOrdem ordem,
            [FromQuery] [SwaggerParameter("Opções de paginação")] LivroPaginacao paginacao)
        {
            var livroPaginado = _repo
                .All
                .AplicarFiltro(filtro)
                .AplicarOrdem(ordem)
                .Select(l => l.ToApi())
                .ToLivroPaginado(paginacao);

            return Ok(livroPaginado);
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult Recuperar(int id)
        {
            var model = _repo.Find(id);

            if (model == null)
                return NotFound(); //404

            return Ok(model);
        }

        [HttpGet]
        [Route("{id}/capa")]
        public IActionResult RecuperarCapa(int id)
        {
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        [HttpPost]
        public IActionResult Incluir([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                _repo.Incluir(livro);
                var uri = Url.Action("Recuperar", new { id = livro.Id });
                return Created(uri, livro); //201
            }

            return BadRequest(ErrorResponse.FromModelState(ModelState)); //400
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Remover(int id)
        {
            var livro = _repo.Find(id);
            if (livro == null)
                return NotFound();

            _repo.Excluir(livro);
            return NoContent(); //204
        }

        [HttpPut]
        public IActionResult Alterar([FromForm] LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repo.Alterar(livro);
                return Ok();
            }

            return BadRequest();
        }
    }
}
