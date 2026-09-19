using Application.DataTransferObject;
using Application.Interfaces;
using Asp.Versioning;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APISolution.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class InscricaoController : ControllerBase
    {
        private readonly IInscricaoServices _iss;        

        public InscricaoController(IInscricaoServices iss)
        {
            _iss = iss;            
        }

        [HttpGet("Inscricoes")]
        [Authorize(Policy = "User")]
        public async Task<ActionResult<ICollection<InscricaoDTO>>> Get()
        {

            var inscricoes = await _iss.GetAll();
            
            var userId = User.FindFirst("userId")?.Value;

            var lista = new List<InscricaoDTO>();


            foreach (var ins in inscricoes)
            {

                if(ins.UsuarioId == userId)
                {
                    lista.Add(ins);
                }
            }

            if(lista.Count == 0)
            {
                return NotFound("Nenhuma Inscrição deste usuário");
            }
            else
            {
                return lista;
            }


            
        }



        [HttpDelete("DeleteInscricao")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> DeleteIns(int id)
        {
            var suc = await _iss.Delete(id);

            if(suc == true)
            {
                return Ok($"Inscrição do evento: {id} cancelada!");
            }
            else
            {
                return BadRequest($"Ocorreu um erro ao deletar a Inscrição com id: {id}");
            }
        }
    }
}
