using Application.DataTransferObject;
using Application.DataTransferObject.IdentityDTO;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUsuarioServices
    {
        public Task<ICollection<UsuarioPrintDTO>> GetAllUsers();
        public Task<ICollection<UsuarioPrintDTO>> GetAtivos();
        public Task<UsuarioPrintDTO> GetIdUsuario(string id);
        public Task<UsuarioPrintDTO> GetNomeUsers(string nome);
        public Task<bool> UpdateUsers(string id, UsuarioPrintDTO dto);
        public Task<bool> DeleteUsers(string id);
        public Task<bool> UpdateSenha (string id, UsuarioSenhaDTO dto);
        public Task<ICollection<UserRolesDTO>> ShowUsersRoles();


        public string NormalizeNome(string nome);

        public Task<ICollection<Usuario>> UsuarioInscricao();
    }
}
