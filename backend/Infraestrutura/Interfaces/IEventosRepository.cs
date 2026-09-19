using Domain.Entities;
using Infraestrutura.ContextRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.Interfaces
{
    public interface IEventosRepository : IRepository<Evento>
    {
        public Task<Evento> GetEventoNome(string nome);
        public Task<ICollection<Evento>> GetEventoInscricao();
        public Task<bool> UpdateADM(int id, double preco, int quantidade);
        public Task<bool> FazerInscricao(int id, Inscricao ins);


    }
}
