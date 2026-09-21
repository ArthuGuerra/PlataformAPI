using Domain.Entities;
using Infraestrutura.BancoContexto;
using Infraestrutura.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
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
            var aux = await _context.Evento.Include(x => x.Inscricoes).ToListAsync();

            if (string.IsNullOrEmpty(aux.ToString()))
            {
                return [];
            }
            else
            {
                return aux;
            }
        }

        
        public async Task<ICollection<Evento>> GetEventosAtivos()
        {
            return await _context.Evento.AsNoTracking().Where(x => x.Ativo).ToListAsync();
        }




        public async Task<Evento> GetEventoNome(string nome)
        {

            var aux = await _context.Evento.FirstOrDefaultAsync(x => x.Nome == nome);

            if (aux != null)
            {
                return aux;
            }
            else
            {
                return null;
            }

        }


        public async Task<bool> UpdateADM(int id, double preco, int quantidade)
        {
            var eve = await _context.Evento.FirstOrDefaultAsync(x => x.Id == id);

            if(eve != null)
            {
                eve.QuantidadeDeKitsDisponiveis = quantidade;
                eve.Preco = preco;

                await _context.SaveChangesAsync();

                return true;
            }
            else
            {
                return false;
            }
                      
        }


        public async Task<bool> FazerInscricao(int id, Inscricao ins)
        {
            var evento = await _context.Evento.FirstOrDefaultAsync(x => x.Id == id);

            if (evento == null) return false;


            var existe = await _context.Inscricao.AnyAsync(x =>
                    x.EventoId == ins.EventoId &&
                    x.UsuarioId == ins.UsuarioId);

            if (existe) return false;

            if (evento.QuantidadeDeKitsDisponiveis <= 0)
            {
                return false;
            }


            ins.DataDeInscricao = DateTime.UtcNow;

            _context.Inscricao.Add(ins);

            evento.QuantidadeDeKitsDisponiveis--;

            evento.Inscricoes.Add(ins);

            await _context.SaveChangesAsync();

            return true;                                        


        }
     
    }
}

