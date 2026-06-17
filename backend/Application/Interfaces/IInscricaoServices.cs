using Application.DataTransferObject;
using Domain.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IInscricaoServices
    {
        public Task<ICollection<Inscricao>> GetAll();
        public Task<InscricaoDTO> Delete(int id);
    }
}
