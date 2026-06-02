using Application.DataTransferObject;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUsuarioServices
    {
        public Task<ICollection<UsuarioCorredorDTO>> GetAllUsers();
        public Task<UsuarioCorredorDTO> GetIdUsers(int id);
        public Task<UsuarioCorredorDTO> GetNomeUsers(string nome);
        public Task<UsuarioCorredorDTO> UpdateUsers(int id, UsuarioCorredorDTO dto);
        public Task<UsuarioCorredorDTO> CreateUsers(UsuarioCorredorDTO dto);
        public Task<UsuarioCorredorDTO> DeleteUsers(int id);
        public string NormalizeNome(string nome);

        public Task<ICollection<Usuario>> UsuarioInscricao();
    }
}
