using Application.DataTransferObject;
using Application.Interfaces;
using Asp.Versioning;
using Domain.Entities;
using Infraestrutura.ContextRepository;
using Infraestrutura.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace APISolution.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class EventoController : ControllerBase
    {
        private readonly IEventoServices _es;
        
        public EventoController(IEventoServices es) 
        {
            _es = es;
        }



        [HttpGet("EventosDTO")]
        public async Task<ActionResult<EventoDTO>> GetAll()
        {           
            var aux = await _es.ListarEventos();

            if (aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound();
            }           
        }




        [HttpGet("{id}")]
        [Authorize(Policy ="Admin")]
        public async Task<ActionResult<EventoDTO>> GetId (int id)
        {           
            var aux = await _es.GetEventoId(id);

            if(aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound(id);
            }             
        }


        [HttpGet("GetNome")]
        public async Task<ActionResult<EventoDTO>> GetNome(string nome)
        {
           
            var aux = await _es.GetEventoNomes(nome);

            if( aux != null )
            {
                return Ok(aux);
            }
            else
            {
                return NotFound(nome);
            }                        
        }


        [HttpPost("CreateEvento")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<EventoDTO>> CreateEventos(EventoDTO evento)
        {
            
            var aux = await _es.CreateEvento(evento);

            if(aux != null)
            {
                return Ok(aux);                                               
            }
            else
            {
                return BadRequest($"Evento com nome: '{_es.NormalizeNome(evento.Nome)}' já existe");
            }            
        }


        [HttpPatch("UpdateADM")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<EventoDTO>> UpdateADM(int id, CreateEventoDTO dto)
        {
           
            var aux = await _es.AtualizarEventoADM(id, dto);

            if(aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return BadRequest(aux);
            }                      
        }


        [HttpPatch("Update")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<EventoDTO>> UpdateEvento(int id, EventoDTO dto)
        {
            
            var aux = await _es.AtualizarEvento(id, dto);

            if(aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return BadRequest(aux);
            }                       
        }



        [HttpDelete("Delete")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<EventoDTO>> DeleteEvento(int id)
        {  
            
            var aux = await _es.DeletarEventos(id);

            if (aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound(aux);
            }                        
        }



        [HttpPatch("Inscricao")]
        [Authorize(Policy = "User")]
        public async Task<ActionResult<EventoDTO>> FazerInscricao(InscricaoDTO dto)
        {
           
            var aux = await _es.Inscrição(dto);

            if (aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return BadRequest(aux);
            }                       
        }


        [HttpGet("EventosInscricoes")]
        [Authorize(Policy = "Admin")]
        public async Task<ActionResult<ICollection<Evento>>> EventoInscricoes()
        {
           
            var aux = await _es.EventoInscricao();

            if(aux != null)
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
