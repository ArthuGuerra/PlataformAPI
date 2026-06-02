using Domain.Entities;
using Infraestrutura.BancoContexto;
using Infraestrutura.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.ContextRepository
{
    public class UsuariosRepositorio : Repositorio<Usuario>, IUsuariosRepository
    {
        private readonly ApiContext _api;
        
        public UsuariosRepositorio(ApiContext api) : base(api)
        {
            _api = api;
        }

        public async Task<Usuario> GetNome(string nome)
        {
            return await _api.Usuario.FirstOrDefaultAsync(x => x.Nome == nome);            
            
        }

        public async Task<ICollection<Usuario>> GetUsuarioInscricao()
        {
            return await _api.Usuario.Include(x => x.Inscricoes).ToListAsync();
        }
    }
}
