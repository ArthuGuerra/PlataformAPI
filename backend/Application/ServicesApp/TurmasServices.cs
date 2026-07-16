using Application.DataTransferObject;
using Application.DataTransferObject.AppDTO;
using Application.InterfacesApp;
using AutoMapper;
using Domain.AppEntities;
using Infraestrutura.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ServicesApp
{
    public class TurmasServices : ITurmasServices
    {
        private readonly IUnitOfWork _api;
        private readonly IMapper _mapper;


        public TurmasServices(IUnitOfWork api, IMapper mapper)
        {
            _api = api;
            _mapper = mapper;            
        }

        public async Task<ICollection<TurmasDTO>> GetTurmas()
        {
            var aux = await _api.TurmasAppRepository.GetAllAsync();

            return _mapper.Map<ICollection<TurmasDTO>>(aux);
        }

        public async Task<TurmasDTO> CriarTurma(TurmasDTO turma)
        {
            var map = _mapper.Map<Turmas>(turma);

            _api.TurmasAppRepository.Create(map);

            await _api.Commit();

            return turma;
        }


        public async Task<TurmasDTO> AtualizarTurma(TurmasDTO turma)
        {

            var map = _mapper.Map<Turmas>(turma);
            
            _api.TurmasAppRepository.Update(map);

            await _api.Commit();

            return turma;
        }
      

        public async Task<bool> RetirarAluno(InscricaoAppDTO dto)
        {
            
            var aux = await _api.InscricaoAppRepository.GetAllAsync();

            var existeUser = aux.Any(x => NormalizeNome(x.UsuarioApp.UserName) == NormalizeNome(dto.Usuario.UserName));

            var map = _mapper.Map<InscricaoApp>(dto);

            if (existeUser)
            {
                _api.InscricaoAppRepository.Delete(map);

                await _api.Commit();

                return true;
            }
            else
            {
                return false;
            }

        }

        public async Task<bool> RetirarTurma(TurmasDTO turma)
        {
            var map = _mapper.Map<Turmas>(turma);

            _api.TurmasAppRepository.Delete(map);

            await _api.Commit();

            return true;
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
