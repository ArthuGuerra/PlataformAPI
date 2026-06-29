using Application.DataTransferObject;
using Domain.Entities;
using Plataforma_Front.Interfaces;
using Plataforma_Front.ViewModels;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Plataforma_Front.Services
{
    public class InscricaoServiceMVC : IInscricaoServiceMVC
    {

        private const string apiEndpoint = "/api/v1/Inscricao/";
        private readonly IHttpClientFactory _client;
        private readonly JsonSerializerOptions _options;
        private readonly InscricaoDTO _inscricaoDTO;
        private ICollection<InscricaoDTO> _inscricoesDTO;
        //private ICollection<EventoInscricaoViewModel> _tudo;


        public InscricaoServiceMVC(IHttpClientFactory client)
        {
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        private static void PutTokenInHeaderAuthorization(string token, HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

    

        public async Task<IEnumerable<InscricaoDTO>> ShowMyIncricoes(string token)
        {
            var cli = _client.CreateClient("APISolution");

            PutTokenInHeaderAuthorization(token, cli);


            using (var response = await cli.GetAsync(apiEndpoint + "Inscricoes"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();

                    _inscricoesDTO = await JsonSerializer.DeserializeAsync<ICollection<InscricaoDTO>>(apiResponse, _options);
                }
                else
                {
                    return _inscricoesDTO ?? Enumerable.Empty<InscricaoDTO>();
                }
            }
            return _inscricoesDTO!;
        }


        public async Task<bool> DeleteMyIncricao(int id, string token)
        {
            var cli = _client.CreateClient("APISolution");

            PutTokenInHeaderAuthorization(token, cli);

            var url = apiEndpoint + $"DeleteInscricao?id={id}";

            //api/v1/Inscricao/DeleteInscricao?id=4

            var response = await cli.DeleteAsync(url);

            return response.IsSuccessStatusCode;
        }
        
    }
}
