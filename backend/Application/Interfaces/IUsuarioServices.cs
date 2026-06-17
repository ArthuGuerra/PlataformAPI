using Application.DataTransferObject;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUsuarioServices
    {
        public Task<ICollection<UsuarioPrintDTO>> GetAllUsers();
        public Task<UsuarioPrintDTO> GetIdUsuario(string id);
        public Task<UsuarioPrintDTO> GetNomeUsers(string nome);
        public Task<UsuarioPrintDTO> UpdateUsers(string id, UsuarioPrintDTO dto);
        public Task<UsuarioPrintDTO> DeleteUsers(string id);
        public Task<UsuarioPrintDTO> UpdateSenha (string id, UsuarioSenhaDTO dto);
        public string NormalizeNome(string nome);

        public Task<ICollection<Usuario>> UsuarioInscricao();
    }
}
