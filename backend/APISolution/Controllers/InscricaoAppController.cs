using Application.DataTransferObject;
using Application.DataTransferObject.AppDTO;
using Application.Interfaces;
using Application.InterfacesApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APISolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InscricaoAppController : ControllerBase
    {
        protected readonly IInscricaoAppServices _services;

        public InscricaoAppController(IInscricaoAppServices services)
        {
            _services = services;
        }

        [HttpGet("InscricoesApp")]
        public async Task<ActionResult<ICollection<InscricaoAppDTO>>> Inscricoes()
        {
            var aux = await _services.GetInscricoes();

            if(aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("FazerInscricao")]
        public async Task<ActionResult<InscricaoAppDTO>> FazerInscricao(InscricaoAppDTO dto)
        {
            var aux = await _services.Inscricao(dto);

            if(aux is true)
            {
                return Created();
            }
            else
            {
                return BadRequest();
            }

        }
    }
}
