using Domain.Entities;
using Infraestrutura.ContextRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.Interfaces
{
    public interface IUsuariosRepository
    {
        public Task<ICollection<Usuario>> GetUsuarioInscricao();
        public Task<ICollection<Usuario>> GetAllAsync();
        public Task<ICollection<Usuario>> GetAllAtivosAsync();
    }
}
