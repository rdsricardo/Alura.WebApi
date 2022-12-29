using Alura.ListaLeitura.HttpClients;
using Alura.ListaLeitura.Modelos;
using Alura.WebAPI.WebApp.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class LivroController : Controller
    {
        private readonly LivrosApiClient api;

        public LivroController(LivrosApiClient api)
        {
            this.api = api;
        }

        [HttpGet]
        public IActionResult Novo()
        {
            return View(new LivroUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Novo(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                await api.PostLivroAsync(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ImagemCapa(int id)
        {
            byte[] img = await api.GetLivroCapaAsync(id);

            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        //Por convenção, entende que é uma action, e como está retornando um objeto, retorna em formato Json.
        public Livro RecuperaLivro(int id)
        {
            var model = new Livro { Id = 9999, Titulo = "Várias formar de fazer uma action" };
            return model;
        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            var model = await api.GetLivroAsync(id);

            if (model == null)
                return NotFound();
            else
                return View(model.ToModel());
        }

        //Action que retorna um objeto em Json
        [HttpGet]
        public IActionResult DetalhesSemHTML(int id)
        {
            var model = new Livro { Id = 111, Titulo = "MVC Action Without HTML" };
            if (model == null)
            {
                return NotFound();
            }
            return Json(model.ToModel());
        }

        //Exemplo para retornar um objeto (por convenção em Json) ou um HTTP Status Code
        public ActionResult<LivroUpload> DetalhesJson(int id)
        {
            var model = RecuperaLivro(id);
            if (model == null)
                return NotFound();

            return model.ToModel();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Detalhes(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                await api.PutLivroAsync(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remover(int id)
        {
            var model = await api.GetLivroAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            await api.DeleteLivroAsync(id);
            return RedirectToAction("Index", "Home");
        }
    }
}