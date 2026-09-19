using Application.DataTransferObject;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace APISolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioServices _us;

        public UsuarioController(IUsuarioServices us)
        {
            _us = us;
        }

        [HttpGet("AllUsuarios")]
        [Authorize(Policy = "Super")]
        public async Task<ActionResult<ICollection<UsuarioPrintDTO>>> GetAll()
        {                                    
            return Ok(await _us.GetAllUsers());                              
        }


        [HttpGet("UsuariosInscricoes")]
        [Authorize(Policy = "Super")]
        public async Task<ActionResult<ICollection<Usuario>>> UsuarioInscricoes()
        {                       
            return Ok(await _us.UsuarioInscricao());                       
        }


        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetId(string id)
        {
          
            var aux = await _us.GetIdUsuario(id);

            if(aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound($"Não foi possível localizar o id: {id}");
            }                        
        }


        [HttpGet("Nome")]
        [Authorize(Policy = "Super")]
        public async Task<IActionResult> GetNome(string nome)
        {
           
            var aux = await _us.GetNomeUsers(nome);

            if( aux != null )
            {
                return Ok(aux);
            }
            else
            {
                return NotFound($"Não foi possível localizar o usuário: {nome}");
            }                        
        }



        [HttpPatch("Update")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> UpdateUser(string id, UsuarioPrintDTO dto)
        {
            
            var aux = await _us.UpdateUsers(id, dto);

            if(aux is true)
            {
                return Ok(aux);
            }
            else
            {
                return BadRequest("Não foi possível atualizar usuário ou não encontrado");
            }                      
        }


        [HttpDelete("Delete")]
        [Authorize(Policy = "Super")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            
            var aux = await _us.DeleteUsers(id);

            if( aux is true)
            {
                return Ok(aux);
            }
            else
            {
                return BadRequest($"Não foi possível deletar o usuário com id: {id} ou não encontrado");
            }                       
        }


        [Authorize(Policy = "User")]
        [HttpPost("TrocarSenha")]
        public async Task<IActionResult> ChangePass(string email, UsuarioSenhaDTO user)
        {
            var aux = await _us.UpdateSenha(email, user);

            if (aux is true)
            {
                return Ok("Senha Atualizada com sucesso");
            }
            else
            {
                return BadRequest("Não foi possível realizar a troca de senha");
            }
        }


        [Authorize(Policy = "Super")]
        [HttpGet("ShowUsers&Roles")]
        public async Task<IActionResult> ShowUserRole()
        {
            return Ok(await _us.ShowUsersRoles());
        }
    }
}
