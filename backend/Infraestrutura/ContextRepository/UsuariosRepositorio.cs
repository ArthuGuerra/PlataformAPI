using Domain.Entities;
using Infraestrutura.BancoContexto;
using Infraestrutura.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.ContextRepository
{
    public class UsuariosRepositorio : IUsuariosRepository
    {
        private readonly ApiContext _api;
        
        public UsuariosRepositorio(ApiContext api)
        {
            _api = api;
        }

        public async Task<ICollection<Usuario>> GetAllAsync()
        {
            return await _api.Set<Usuario>().AsNoTracking().ToListAsync();
        }       

        public async Task<ICollection<Usuario>> GetUsuarioInscricao()
        {
            return await _api.Usuario.Include(x => x.Inscricoes).ToListAsync();
        }

        
    }
}
