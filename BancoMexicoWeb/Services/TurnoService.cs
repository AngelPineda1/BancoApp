using BancoAPI.Models.Dtos;
using System.Net.Http.Headers;

namespace BancoMexicoWeb.Services
{
    public class TurnoService
    {

        private readonly HttpClient _httpClient;
        private readonly Uri _url = new("https://BancoMexicoAPI.websitos256.com/");


        public TurnoService()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = _url
            };

        }


        public async Task<string?> Login(LoginDto loginDto)
        {

            var response = await _httpClient.PostAsJsonAsync("api/login", loginDto);

            response.EnsureSuccessStatusCode();
           
            if(response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync() ?? "";
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                return token;
            }

            return null;
        }

    }
}
