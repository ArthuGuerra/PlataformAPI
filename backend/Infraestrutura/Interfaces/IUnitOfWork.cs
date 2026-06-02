using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.Interfaces
{
    public interface IUnitOfWork
    {

        public IUsuariosRepository UsuariosRepository { get; set; }
        public IEventosRepository EventosRepository { get; set; }
        public IInscricaoRepository InscricaoRepository { get; set; }

        public Task Commit();
        public Task Disposes();
    }
}
