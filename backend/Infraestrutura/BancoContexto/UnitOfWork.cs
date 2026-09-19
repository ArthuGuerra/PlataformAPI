using Domain.Entities;
using Infraestrutura.ContextRepository;
using Infraestrutura.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infraestrutura.BancoContexto
{
    public class UnitOfWork : IUnitOfWork
    {
        private  ApiContext? _api;
        private IUsuariosRepository _usuarios;
        private IEventosRepository _eventos;
        private IInscricaoRepository _inscricao;
        private IInscricaoAppRepository _inscricaoApp;
        private ITurmasRepository _turmas;


        public UnitOfWork (ApiContext api )
        {
            _api = api;           
        }

        public IUsuariosRepository UsuariosRepository 
        {
            get 
            {
                return _usuarios = _usuarios ?? new UsuariosRepositorio(_api);
            }

            set { _usuarios = value; }
                   
        
        }
        public IEventosRepository EventosRepository 
        { 
            get
            {
                return _eventos = _eventos ?? new EventosRepositorio(_api); 
            }

            set { _eventos = value; }
        
        }

        public IInscricaoRepository InscricaoRepository
        {
            get
            {
                return _inscricao = _inscricao ?? new InscricaoRepository(_api);
            }

            set { _inscricao = value; }
        }

        public IInscricaoAppRepository InscricaoAppRepository
        {
            get
            {
                return _inscricaoApp = _inscricaoApp ?? new InscricaoAppRepository(_api);
            }

            set { _inscricaoApp = value;  }
        }

        public ITurmasRepository TurmasAppRepository
        {
            get
            {
                return _turmas = _turmas ?? new TurmasRepository(_api);
            }

            set { _turmas = value; }
        }




        public async Task Commit()
        {
           try
            {
                await _api.SaveChangesAsync();
            }
            catch(Exception)
            {
                throw new Exception("ERRO AO SALAVR NO BANCO - SaveChances");
            }

        }

        public async Task Disposes()
        {
            try
            {
                await _api.DisposeAsync();
            }
            catch(Exception)
            {
                throw new Exception("Erro ao liberar dados do context");
            }
        }
    }
}
