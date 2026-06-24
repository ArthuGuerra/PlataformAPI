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
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ICollection<UsuarioPrintDTO>>> GetAll()
        {
            
            var aux = await _us.GetAllUsers();

            if(aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound(aux);
            }                       
        }


        [HttpGet("UsuariosInscricoes")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ICollection<Usuario>>> UsuarioInscricoes()
        {
            
            var aux = await _us.UsuarioInscricao();

            if (aux == null)
            {
                return NotFound(aux);
            }
            else
            {
                return Ok(aux);
            }            
        }


        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<UsuarioPrintDTO>> GetId(string id)
        {
          
            var aux = await _us.GetIdUsuario(id);

            if(aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound(aux);
            }                        
        }


        [HttpGet("Nome")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<UsuarioPrintDTO>> GetNome(string nome)
        {
           
            var aux = await _us.GetNomeUsers(nome);

            if( aux != null )
            {
                return Ok(aux);
            }
            else
            {
                return NotFound(aux);
            }                        
        }



        [HttpPatch("Update")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<UsuarioPrintDTO>> UpdateUser(string id, UsuarioPrintDTO dto)
        {
            
            var aux = await _us.UpdateUsers(id, dto);

            if(aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound(aux);
            }                      
        }


        [HttpDelete("Delete")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<UsuarioPrintDTO>> DeleteUser(string id)
        {
            
            var aux = await _us.DeleteUsers(id);

            if( aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound(aux);
            }                       
        }
    }
}
