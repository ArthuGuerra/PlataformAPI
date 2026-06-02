using Domain.Entities;
using Infraestrutura.BancoContexto;
using Infraestrutura.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.ContextRepository
{
    public class InscricaoRepository: Repositorio<Inscricao>, IInscricaoRepository
    {
        private readonly ApiContext _context;
        public InscricaoRepository(ApiContext context) : base(context)
        {
            _context = context;
        }

        //public async Task<ICollection<Inscricao>> Get()
        //{
        //    return await _context.Inscricao.ToListAsync();
        //}
    }
}
