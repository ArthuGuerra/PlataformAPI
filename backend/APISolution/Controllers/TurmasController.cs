using Application.DataTransferObject;
using Application.DataTransferObject.AppDTO;
using Application.InterfacesApp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APISolution.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurmasController : ControllerBase
    {
        private readonly ITurmasServices _services;

        public TurmasController(ITurmasServices services)
        {
            _services = services;
        }

        [HttpGet("Turmas")]
        public async Task<ActionResult<ICollection<TurmasDTO>>> Turmas()
        {
            var aux = await _services.GetTurmas();

            if(aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost("CriarTurma")]
        public async Task<ActionResult<TurmasDTO>> CreateTurma(TurmasDTO dto)
        {
            var aux = await _services.CriarTurma(dto);

            if(aux != null)
            {
                return Created();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("RemoverTurma")]
        public async Task<ActionResult<TurmasDTO>> RemoverTurma(TurmasDTO turma)
        {
            var aux = await _services.RetirarTurma(turma);

            if(aux is true)
            {
                return Ok(aux);
            }
            else
            {
                return BadRequest();
            }
        }
        
        [HttpDelete("RemoverAluno")]
        public async Task<ActionResult<TurmasDTO>> TirarAluno(InscricaoAppDTO dto)
        {
            var aux = await _services.RetirarAluno(dto);

            if( aux is true)
            {
                return Ok(aux);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPatch("AtualizarTurma")]
        public async Task<ActionResult<TurmasDTO>> UpdateTurma(TurmasDTO turma)
        {
            var aux = await _services.AtualizarTurma(turma);

            if(aux == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(aux);
            }
        }

        

    }
}
