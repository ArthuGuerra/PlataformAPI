using Domain.AppEntities;
using Infraestrutura.BancoContexto;
using Infraestrutura.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.ContextRepository
{
    public class InscricaoAppRepository : Repositorio<InscricaoApp>, IInscricaoAppRepository
    {
        private ApiContext _options;

        public InscricaoAppRepository(ApiContext options) :base(options)
        {
            _options = options;
        }
    }
}
