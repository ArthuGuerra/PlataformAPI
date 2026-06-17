using Application.DataTransferObject;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infraestrutura.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Application.Services
{
    public class UsuariosServices : IUsuarioServices
    {
        private readonly IUnitOfWork _api;
        private readonly IMapper _mapper;
        private readonly UserManager<Usuario> _user;

        public UsuariosServices(IUnitOfWork api, IMapper mapper, UserManager<Usuario> user)
        {
            _api = api;
            _mapper = mapper;
            _user = user;
        }


        public async Task<ICollection<UsuarioPrintDTO>> GetAllUsers()
        {
            var aux = await _api.UsuariosRepository.GetAllAsync();

            return _mapper.Map<ICollection<UsuarioPrintDTO>>(aux);

            
        }

        public async Task<UsuarioPrintDTO> GetNomeUsers(string nome)
        {
            var aux = await _user.FindByNameAsync(nome);

            return _mapper.Map<UsuarioPrintDTO>(aux);
        }


        
        public async Task<UsuarioPrintDTO> GetIdUsuario(string id)
        {
            var aux = await _user.FindByIdAsync(id);

            return _mapper.Map<UsuarioPrintDTO>(aux);
        }      


        
        public async Task<UsuarioPrintDTO> UpdateUsers(string id, UsuarioPrintDTO dto)
        {

            var aux = await _user.FindByIdAsync(id);

            if(aux != null)
            {
                aux.UserName = dto.UserName;
                aux.Email = dto.Email;                
            }

            var result = await _user.UpdateAsync(aux);

            if (!result.Succeeded)
            {
                foreach (var erro in result.Errors)
                {
                    Debug.WriteLine(erro.Description);
                }
                return null;
            }

            return _mapper.Map<UsuarioPrintDTO>(aux);
        }



        public async Task<UsuarioPrintDTO> DeleteUsers(string id)
        {
            var aux = await _user.FindByIdAsync(id);

            if(aux != null)
            {
                await _user.DeleteAsync(aux);

                return _mapper.Map<UsuarioPrintDTO>(aux);
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

        public async Task<UsuarioPrintDTO> UpdateSenha(string id,UsuarioSenhaDTO dto)
        {
            var user = await _user.FindByIdAsync(id);

            if(user == null)
            {
                return null;
            }
            else
            {

                var result = await _user.ChangePasswordAsync(
                    user,
                    dto.SenhaAtual,
                    dto.NewSenha
                    );

                return _mapper.Map<UsuarioPrintDTO>(dto);
                
            }
        }
    }
}
