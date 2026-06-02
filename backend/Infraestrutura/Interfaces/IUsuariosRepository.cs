using Domain.Entities;
using Infraestrutura.ContextRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.Interfaces
{
    public interface IUsuariosRepository : IRepository<Usuario>
    {
        public Task<Usuario> GetNome(string nome);

        public Task<ICollection<Usuario>> GetUsuarioInscricao();
    }
}
