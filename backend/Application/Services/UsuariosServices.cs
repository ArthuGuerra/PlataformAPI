using Application.DataTransferObject;
using Application.DataTransferObject.IdentityDTO;
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
        private readonly RoleManager<IdentityRole> _role;

        public UsuariosServices(IUnitOfWork api, IMapper mapper, UserManager<Usuario> user, RoleManager<IdentityRole> role)
        {
            _api = api;
            _mapper = mapper;
            _user = user;
            _role = role;
        }


        public async Task<ICollection<UsuarioPrintDTO>> GetAllUsers()
        {
            var aux = await _api.UsuariosRepository.GetAllAsync();

            if(aux.Count == 0)
            {
                return [];
            }
            else
            {
                return _mapper.Map<ICollection<UsuarioPrintDTO>>(aux);
            }

        }

        public async Task<UsuarioPrintDTO> GetNomeUsers(string nome)
        {
            var aux = await _user.FindByNameAsync(nome);

            if(aux != null)
            {
                return _mapper.Map<UsuarioPrintDTO>(aux);
            }
            else
            {
                return null;
            }

        }


        
        public async Task<UsuarioPrintDTO> GetIdUsuario(string id)
        {
            var aux = await _user.FindByIdAsync(id);

            if(aux != null)
            {
                return _mapper.Map<UsuarioPrintDTO>(aux);
            }
            else
            {
                return null;
            }

        }


        public async Task<bool> UpdateUsers(
     string id,
     UsuarioPrintDTO dto)
        {
            var user = await _user.FindByIdAsync(id);

            if (user is null)
            {
                return false;
            }

            var userNameExistente = await _user.FindByNameAsync(dto.UserName);

            if (userNameExistente is not null &&
                userNameExistente.Id != user.Id)
            {
                return false;
            }

            var emailExistente = await _user.FindByEmailAsync(dto.Email);

            if (emailExistente is not null &&
                emailExistente.Id != user.Id)
            {
                return false;
            }

            user.UserName = dto.UserName;
            user.Email = dto.Email;

            var result = await _user.UpdateAsync(user);

            return result.Succeeded;
        }



        public async Task<bool> DeleteUsers(string id)
        {
            var aux = await _user.FindByIdAsync(id);

            if(aux != null)
            {
                await _user.DeleteAsync(aux);

                return true;
            }
            else
            {
                return false;
            }                              
        }


        public async Task<ICollection<UserRolesDTO>> ShowUsersRoles()
        {
            var aux = await _api.UsuariosRepository.GetAllAsync();

            if (aux.Count == 0) return [];


            var result = new List<UserRolesDTO>();


            foreach (var user in aux)
            {
                var roles = await _user.GetRolesAsync(user);

                result.Add(new UserRolesDTO
                {
                    UserName = user.UserName ?? string.Empty,
                    Roles = roles
                });
            }

            return result;
        }



        public string NormalizeNome(string? nome)
        {
            if(string.IsNullOrEmpty(nome))
            {
                return string.Empty;
            }

            return String.Join(" ", nome
                .Trim().
                ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }



        public async Task<ICollection<Usuario>> UsuarioInscricao()
        {
            var aux = await _api.UsuariosRepository.GetUsuarioInscricao();

            if(aux.Count == 0)
            {
                return [];
            }
            else
            {
                return aux;
            }
        }

        public async Task<bool> UpdateSenha(string email,UsuarioSenhaDTO dto)
        {
            var user = await _user.FindByEmailAsync(email);

            if(user != null)
            {
              
                await _user.ChangePasswordAsync(
                    user,
                    dto.SenhaAtual,
                    dto.NewSenha
              );

                return true;
            }
            else
            {
                return false;
            }
           
          
            
        }     
    }
}
