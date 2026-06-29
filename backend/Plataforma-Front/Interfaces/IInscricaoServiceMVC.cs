using Application.DataTransferObject;
using Domain.Entities;

namespace Plataforma_Front.Interfaces
{
    public interface IInscricaoServiceMVC
    {
        Task<IEnumerable<InscricaoDTO>> ShowMyIncricoes(string token);
        Task<bool> DeleteMyIncricao(int id, string token);
    }
}
