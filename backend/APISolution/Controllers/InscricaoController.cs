using Application.DataTransferObject;
using Application.Interfaces;
using Asp.Versioning;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        //[Authorize(Policy = "User")]
        public async Task<ActionResult<ICollection<InscricaoDTO>>> Get()
        {
            
            var aux = await _iss.GetAll();

            if(aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound();
            }                       
        }

        [HttpDelete("DeleteInscricao")]
        //[Authorize(Policy = "User")]
        public async Task<ActionResult<InscricaoDTO>> DeleteIns(int id)
        {
            return await _iss.Delete(id);
        }
    }
}
