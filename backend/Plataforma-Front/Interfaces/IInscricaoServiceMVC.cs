using Application.DataTransferObject;
using Domain.Entities;

namespace Plataforma_Front.Interfaces
{
    public interface IInscricaoServiceMVC
    {
        Task<IEnumerable<InscricaoDTO>> ShowMyIncricoes();
        Task<bool> DeleteMyIncricao(int id);
    }
}
