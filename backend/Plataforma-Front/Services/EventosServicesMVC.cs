using Application.DataTransferObject;
using Application.Interfaces;
using Infraestrutura.Interfaces;
using Plataforma_Front.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Plataforma_Front.Services
{
    public class EventosServicesMVC : IEventosServicesMVC
    {

        private const string apiEndpoint = "/api/v1/Evento/";
        private readonly IHttpClientFactory _client;
        private readonly JsonSerializerOptions _options;
        private EventoDTO _eventoDTO;
        private ICollection<EventoDTO> _eventosDTO;


        public EventosServicesMVC(IHttpClientFactory client)
        {
            _client = client;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }


        private static void PutTokenInHeaderAuthorization(string token, HttpClient client)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<IEnumerable<EventoDTO>> GetEventos()
        {
            var cli = _client.CreateClient("APISolution");            
            
            using(var response = await cli.GetAsync(apiEndpoint +"EventosDTO"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadAsStreamAsync();

                    _eventosDTO = await JsonSerializer.DeserializeAsync<ICollection<EventoDTO>>(apiResponse, _options);
                }
                else
                {
                    return _eventosDTO ?? Enumerable.Empty<EventoDTO>();
                }                
            }
            return _eventosDTO!;
        }


        public async Task<EventoDTO?> GetEventoNome(string nome)
        {
            var cli = _client.CreateClient("APISolution");

            var url = apiEndpoint + $"GetNome?nome={Uri.EscapeDataString(nome)}";

            try
            {
                var response = await cli.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                _eventoDTO = await response.Content.ReadFromJsonAsync<EventoDTO>();
                return _eventoDTO;
            }
            catch (HttpRequestException)
            {
                return null;
            }
            
        }

        public async Task<bool> FazerInscricaoService(InscricaoDTO dto, string token)
        {
            try
            {
                var cli = _client.CreateClient("APISolution");

                PutTokenInHeaderAuthorization(token, cli);

                using var response =
                    await cli.PatchAsJsonAsync("api/v1/Evento/Inscricao", dto);

                return response.IsSuccessStatusCode;
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }




    }
}
