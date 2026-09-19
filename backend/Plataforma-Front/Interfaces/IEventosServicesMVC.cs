using Application.DataTransferObject;
using Domain.Entities;

namespace Plataforma_Front.Interfaces
{
    public interface IEventosServicesMVC
    {
        Task<IEnumerable<EventoDTO>> GetEventos();
        Task<EventoDTO> GetEventoNome(string nome);
        Task<bool> FazerInscricaoService(int id, InscricaoDTO dto, string token);
        Task<bool> CriarEvento(EventoDTO dto, string token);
        Task<bool> UpdateEventoADM(Evento dto, string token);
        Task<bool> UpdateEvento(EventoDTO dto, string token);
        Task<bool> DeleteEvento(int id, string token);
    }
}
