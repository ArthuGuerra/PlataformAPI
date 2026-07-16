using Application.DataTransferObject;
using Application.DataTransferObject.AppDTO;
using Application.DataTransferObject.IdentityDTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.InterfacesApp
{
    public interface IInscricaoAppServices
    {
        public Task<ICollection<InscricaoAppDTO>> GetInscricoes();
        public Task<bool> Inscricao(InscricaoAppDTO dto);
    }
}
