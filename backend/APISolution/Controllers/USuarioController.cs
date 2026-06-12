using Application.DataTransferObject;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace APISolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class USuarioController : ControllerBase
    {
        private readonly IUsuarioServices _us;

        public USuarioController(IUsuarioServices us)
        {
            _us = us;
        }

        [HttpGet("AllUsuarios")]   
        public async Task<ActionResult<ICollection<UsuarioCorredorDTO>>> GetAll()
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
        public async Task<ActionResult<UsuarioCorredorDTO>> GetId(int id)
        {
          
            var aux = await _us.GetIdUsers(id);

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
        public async Task<ActionResult<UsuarioCorredorDTO>> GetNome(string nome)
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



        [HttpPost("Create")]
        public async Task<ActionResult<UsuarioCorredorDTO>> CreateUser(UsuarioCorredorDTO dto)
        {
           
            var aux = await _us.CreateUsers(dto);

            if(aux != null)
            {
                return Created();
            }
            else
            {
                return BadRequest($"O evento: '{_us.NormalizeNome(dto.Nome)}' já existe");
            }                        
        }



        [HttpPatch("Update")]
        public async Task<ActionResult<UsuarioCorredorDTO>> UpdateUser(int id, UsuarioCorredorDTO dto)
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
        public async Task<ActionResult<UsuarioCorredorDTO>> DeleteUser(int id)
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
