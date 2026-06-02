using Domain.Entities;
using Infraestrutura.BancoContexto;
using Infraestrutura.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.ContextRepository
{
    public class EventosRepositorio : Repositorio<Evento>, IEventosRepository
    {
        private readonly ApiContext _context;

        public EventosRepositorio(ApiContext context) : base(context)
        {
            _context = context;
        }


        public async Task<ICollection<Evento>> GetEventoInscricao()
        {
            return await _context.Evento.Include(x => x.Inscricoes).ToListAsync();
        }




        public async Task<Evento> GetEventoNome(string nome)
        {

            var aux = await _context.Evento.FirstOrDefaultAsync(x => x.Nome == nome);

            return aux;

        }   
    }
}

