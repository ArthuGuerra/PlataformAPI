using Application.DataTransferObject;
using Application.DataTransferObject.AppDTO;
using Application.InterfacesApp;
using Infraestrutura.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Domain.AppEntities;
using Domain.Entities;
using Application.Interfaces;

namespace Application.ServicesApp
{
    public class InscricaoAppServices : IInscricaoAppServices
    {
        private readonly IUnitOfWork _api;
        protected readonly IMapper _mapper;
                

        public InscricaoAppServices(IUnitOfWork api, IMapper mapper)
        {
            _api = api;
            _mapper = mapper;
        }

        public async Task<ICollection<InscricaoAppDTO>> GetInscricoes()
        {
            var aux = await _api.InscricaoAppRepository.GetAllAsync();

            return _mapper.Map<ICollection<InscricaoAppDTO>>(aux);
        }

        public async Task<bool> Inscricao(InscricaoAppDTO dto)
        {
            var ev = await _api.TurmasAppRepository.GetIdAsync(dto.Turmas.Id);

            if(ev == null)
            {
                return false;
            }

            var aux = await _api.InscricaoAppRepository.GetAllAsync();

            var existeUser = aux.Any(x => NormalizeNome(x.UsuarioApp.UserName) == NormalizeNome(dto.Usuario.UserName));

            var existeEmail = aux.Any(x => NormalizeNome(x.UsuarioApp.Email) == NormalizeNome(dto.Usuario.Email));

            

            if(dto.Turmas.QuantidadeDeAlunos <= 0)
            {
                throw new Exception("Inscrição inválida. Turma cheia.");
            }

            if(existeUser || existeEmail)
            {
                throw new Exception("Inscrição inválida");
            }
            else
            {
               

                var tm = _mapper.Map<Turmas>(dto.Turmas);
                var us = _mapper.Map<Usuario>(dto.Usuario);

                tm.QuantidadeDeAlunos--;


                var obj = new InscricaoApp()
                {
                    UsuarioApp = us,
                    Turma = tm
                };

                _api.InscricaoAppRepository.Create(obj);

                await _api.Commit();

                return true;
                  
            }

        }



        public string NormalizeNome(string nome)
        {
            return string.Join(" ", nome
                .Trim()
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
