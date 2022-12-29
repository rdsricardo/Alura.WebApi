using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivrosApiClient
    {
        private readonly HttpClient httpClient;
        private readonly IHttpContextAccessor accessor;


        public LivrosApiClient(HttpClient httpClient, IHttpContextAccessor accessor)
        {
            this.httpClient = httpClient;
            this.accessor = accessor;
        }

        private void AddBearerToken()
        {
            var token = accessor.HttpContext.User.Claims.First(c => c.Type == "Token").Value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            AddBearerToken();
            var response = await httpClient.GetAsync($"Livros/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<byte[]> GetLivroCapaAsync(int id)
        {
            AddBearerToken();
            var response = await httpClient.GetAsync($"Livros/{id}/capa");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public async Task DeleteLivroAsync(int id)
        {
            AddBearerToken();
            var response = await httpClient.DeleteAsync($"Livros/{id}");
            response.EnsureSuccessStatusCode();
        }

        private string EncapsuleWithQuotationMarks(string value)
        {
            return $"\"{value}\"";
        }

        private HttpContent CreateMultpartFormDataContent(LivroUpload model)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Titulo), EncapsuleWithQuotationMarks("titulo"));
            content.Add(new StringContent(model.Lista.ParaString()), EncapsuleWithQuotationMarks("lista"));

            if (model.Subtitulo != null)
                content.Add(new StringContent(model.Subtitulo), EncapsuleWithQuotationMarks("subtitulo"));

            if (model.Resumo != null)
                content.Add(new StringContent(model.Resumo), EncapsuleWithQuotationMarks("resumo"));

            if (model.Autor != null)
                content.Add(new StringContent(model.Autor), EncapsuleWithQuotationMarks("autor"));


            if (model.Id > 0)
                content.Add(new StringContent(model.Id.ToString()), EncapsuleWithQuotationMarks("id"));

            if (model.Capa != null)
            {
                var imageContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imageContent.Headers.Add("content-type", "image/png");
                content.Add(imageContent, EncapsuleWithQuotationMarks("capa"), EncapsuleWithQuotationMarks("capa.png"));
            }

            return content;

        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            //var content = new StringContent(JsonConvert.SerializeObject(model, Formatting.Indented), Encoding.UTF8);
            AddBearerToken();
            var content = CreateMultpartFormDataContent(model);
            var response = await httpClient.PostAsync("livros", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task PutLivroAsync(LivroUpload model)
        {
            AddBearerToken();
            var content = CreateMultpartFormDataContent(model);
            var response = await httpClient.PutAsync("livros", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            AddBearerToken();
            var response = await httpClient.GetAsync($"ListaLeitura/{tipo}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsAsync<Lista>();
        }
    }
}