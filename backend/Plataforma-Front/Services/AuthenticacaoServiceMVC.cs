using Application.DataTransferObject;
using Application.DataTransferObject.IdentityDTO;
using Azure;
using Plataforma_Front.Interfaces;
using Plataforma_Front.ViewModels;
using System.Text;
using System.Text.Json;

namespace Plataforma_Front.Services
{
    public class AuthenticacaoServiceMVC : IAuthenticacao
    {
        private readonly IHttpClientFactory _client;
        const string apiEndpoint = "api/v1/Auth/Login";
        const string apiEndpointRegis = "api/v1/Auth/Cadastro";
        private readonly JsonSerializerOptions _options;
        private TokenViewModel _token;


        public AuthenticacaoServiceMVC(IHttpClientFactory client)
        {
            _client = client;
            _options = new JsonSerializerOptions {PropertyNameCaseInsensitive = true };
        }


        public async Task<TokenViewModel> AutenticaUsuario(UsuarioViewModel model)
        {
            var client = _client.CreateClient("APIAuth");

            var usuario = JsonSerializer.Serialize(model);
            StringContent content = new StringContent(usuario, Encoding.UTF8,"application/json");

            using( var response = await client.PostAsync(apiEndpoint, content))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();

                     _token = await JsonSerializer.DeserializeAsync<TokenViewModel>(apiResponse, _options);
                }
                else
                {
                    return null;
                }
            }
            return _token;
        }

        public async Task RegisterUsuario(RegisterModelDTO model)
        {
            var client = _client.CreateClient("APIAuth");

            var registro = JsonSerializer.Serialize(model);
            StringContent content = new StringContent(registro, Encoding.UTF8, "application/json");


            using var response = await client.PostAsync(apiEndpointRegis, content);

            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();

                throw new Exception(
                    $"Erro ao registrar o usuário: {erro}");
            }

        }
       
    }
}
