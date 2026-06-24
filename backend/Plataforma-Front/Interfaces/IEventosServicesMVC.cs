using Application.DataTransferObject;

namespace Plataforma_Front.Interfaces
{
    public interface IEventosServicesMVC
    {
        Task<IEnumerable<EventoDTO>> GetEventos();
        Task<EventoDTO> GetEventoNome(string nome);
        Task<bool> FazerInscricaoService(InscricaoDTO dto);
    }
}
