using Application.DataTransferObject;
using Application.Interfaces;
using Domain.Entities;
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

        public async Task<bool> FazerInscricaoService(int id, InscricaoDTO dto, string token)
        {
            try
            {
                var cli = _client.CreateClient("APISolution");

                PutTokenInHeaderAuthorization(token, cli);

                using var response =
                    await cli.PatchAsJsonAsync($"api/v1/Evento/Inscricao/{id}", dto);

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

        public async Task<bool> CriarEvento(EventoDTO dto, string token)
        {
            var cli = _client.CreateClient("APISolution");

            PutTokenInHeaderAuthorization(token, cli);
            
            try
            {
                using var response = await cli.PostAsJsonAsync("api/v1/Evento/CreateEvento", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                else
                {
                    return true;
                }
                            
            }
            catch (HttpRequestException)
            {
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }



        }

        public async Task<bool> UpdateEventoADM(Evento dto, string token)
        {
            var cli = _client.CreateClient("APISolution");

            PutTokenInHeaderAuthorization(token, cli);

            try
            {
                using var response = await cli.PatchAsJsonAsync($"api/v1/Evento/UpdateADM?id={dto.Id}", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                else
                {
                    return true;
                }

                
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }




        

        public async Task<bool> UpdateEvento(EventoDTO dto, string token)
        {
            var cli = _client.CreateClient("APISolution");

            PutTokenInHeaderAuthorization(token, cli);

            try
            {
                using var response = await cli.PatchAsJsonAsync($"api/v1/Evento/Update?id={dto.Id}", dto);

                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                else
                {
                    return true;
                }

               
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> DeleteEvento(int id, string token)
        {
            var cli = _client.CreateClient("APISolution");

            PutTokenInHeaderAuthorization(token, cli);

            try
            {
                using var response = await cli.GetAsync($"api/v1/Evento/{id}");

                if (response != null)
                {
                    var del = await cli.DeleteAsync($"api/v1/Evento/Delete?id={id}");

                    return del.IsSuccessStatusCode;
                }
                else
                {
                    return false;
                }
            }catch (HttpRequestException)
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
