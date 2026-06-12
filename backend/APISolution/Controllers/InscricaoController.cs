using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APISolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscricaoController : ControllerBase
    {
        private readonly IInscricaoServices _iss;

        public InscricaoController(IInscricaoServices iss)
        {
            _iss = iss;
        }

        [HttpGet("Inscricoes")]
        public async Task<ActionResult<ICollection<Inscricao>>> Get()
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
    }
}
