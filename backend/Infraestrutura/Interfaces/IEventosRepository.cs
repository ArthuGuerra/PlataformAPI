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
    }
}
