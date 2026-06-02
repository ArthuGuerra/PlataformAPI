using Application.DataTransferObject;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infraestrutura.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class UsuariosServices : IUsuarioServices
    {
        private readonly IUnitOfWork _api;
        private readonly IMapper _mapper;

        public UsuariosServices(IUnitOfWork api, IMapper mapper)
        {
            _api = api;
            _mapper = mapper;
        }


        public async Task<ICollection<UsuarioCorredorDTO>> GetAllUsers()
        {
            var aux = await _api.UsuariosRepository.GetAllAsync();

            return _mapper.Map<ICollection<UsuarioCorredorDTO>>(aux);

            
        }

        public async Task<UsuarioCorredorDTO> GetNomeUsers(string nome)
        {
            var aux = await _api.UsuariosRepository.GetNome(nome);

            return _mapper.Map<UsuarioCorredorDTO>(aux);
        }


        
        public async Task<UsuarioCorredorDTO> GetIdUsers(int id)
        {
            var aux = await _api.UsuariosRepository.GetIdAsync(id);

            return _mapper.Map<UsuarioCorredorDTO>(aux);
        }      


        
        public async Task<UsuarioCorredorDTO> CreateUsers(UsuarioCorredorDTO dto)
        {
            var ev = await _api.UsuariosRepository.GetAllAsync();

            var norma = NormalizeNome(dto.Nome);

            var existe = ev.Any(x => NormalizeNome(x.Nome) == norma);

            if (existe)
            {
                return null;
            }
            else
            {
                var map = _mapper.Map<Usuario>(dto);
                _api.UsuariosRepository.Create(map);

                await _api.Commit();

                return _mapper.Map<UsuarioCorredorDTO>(map);
            }                           
        }



        public async Task<UsuarioCorredorDTO> UpdateUsers(int id, UsuarioCorredorDTO dto)
        {

            var aux = await _api.UsuariosRepository.GetIdAsync(id);

            if(aux != null)
            {
                aux.Nome = dto.Nome;
                aux.Email = dto.Email;                
            }

            _api.UsuariosRepository.Update(aux);

            await _api.Commit();            
            
            return _mapper.Map<UsuarioCorredorDTO>(aux);
        }



        public async Task<UsuarioCorredorDTO> DeleteUsers(int id)
        {
            var aux = await _api.UsuariosRepository.GetIdAsync(id);

            if(aux != null)
            {
                _api.UsuariosRepository.Delete(aux);

                await _api.Commit();

                return _mapper.Map<UsuarioCorredorDTO>(aux);
            }
            else
            {
                return null;
            }

           
        }

       

        public string NormalizeNome(string nome)
        {
            return String.Join(" ", nome
                .Trim().
                ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        public async Task<ICollection<Usuario>> UsuarioInscricao()
        {
            var aux = await _api.UsuariosRepository.GetUsuarioInscricao();

            if(aux == null)
            {
                return null;
            }
            else
            {
                return aux;
            }
        }
    }
}
