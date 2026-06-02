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
            try
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
            catch (Exception ex)
            {
                throw new Exception("Falha ao encontrar a lista de usuários");
                
                //return BadRequest(ex.ToString());
            }
        }


        [HttpGet("UsuariosInscricoes")]
        public async Task<ActionResult<ICollection<Usuario>>> UsuarioInscricoes()
        {
            try
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
            } catch(Exception ex)
            {
                throw new Exception("falha ao mostrar coleção de usuarios");
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioCorredorDTO>> GetId(int id)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Falha ao encontrar o id: {id} ");
            }
        }


        [HttpGet("Nome")]
        public async Task<ActionResult<UsuarioCorredorDTO>> GetNome(string nome)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Falha ao encontrar o usuario com nome: {nome}");
            }
        }



        [HttpPost("Create")]
        public async Task<ActionResult<UsuarioCorredorDTO>> CreateUser(UsuarioCorredorDTO dto)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Falha ao criar usuario");
            }
        }



        [HttpPatch("Update")]
        public async Task<ActionResult<UsuarioCorredorDTO>> UpdateUser(int id, UsuarioCorredorDTO dto)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Falha ao atualizar Usuario");
            }
        }


        [HttpDelete("Delete")]
        public async Task<ActionResult<UsuarioCorredorDTO>> DeleteUser(int id)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception($"Falha ao deleter user com id: {id}");
            }
        }
    }
}
