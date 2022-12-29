using Alura.ListaLeitura.Seguranca;
using System.Net.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string Token { get; set; }
    }

    public class AuthApiClient
    {
        private readonly HttpClient httpClient;

        public AuthApiClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<LoginResult> PostLoginAsync(LoginModel model)
        {
            var response = await httpClient.PostAsJsonAsync("login", model);

            var loginResult = new LoginResult
            {
                Succeeded = response.IsSuccessStatusCode,
                Token = await response.Content.ReadAsStringAsync()
            };

            return loginResult;
        }
    }
}