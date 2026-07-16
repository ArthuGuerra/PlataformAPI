using Domain.AppEntities;
using Infraestrutura.BancoContexto;
using Infraestrutura.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.ContextRepository
{
    public class TurmasRepository : Repositorio<Turmas>, ITurmasRepository
    {
        private ApiContext _options;

        public TurmasRepository(ApiContext options) :base(options)
        {
            _options = options;
        }
    }
}
