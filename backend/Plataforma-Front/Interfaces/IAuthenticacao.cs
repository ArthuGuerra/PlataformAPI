using Application.DataTransferObject;
using Application.DataTransferObject.IdentityDTO;
using Plataforma_Front.ViewModels;

namespace Plataforma_Front.Interfaces
{
    public interface IAuthenticacao
    {
        public Task<TokenViewModel> AutenticaUsuario(UsuarioViewModel model);
        public Task RegisterUsuario(RegisterModelDTO model);
    }
}
