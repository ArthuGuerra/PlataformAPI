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
using System.Collections.ObjectModel;

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
        public async Task<ActionResult<Collection<EventoDTO>>> GetAll()
        {                                   
            return Ok(await _es.ListarEventos());
                   
        }


        [HttpGet("EventosDTO/Ativos")]
        public async Task<ActionResult<Collection<EventoDTO>>> GetAtivos()
        {
            return Ok(await _es.EventosAtivos());

        }




        [HttpGet("{id}")]
        [Authorize(Policy = "Super")]
        public async Task<IActionResult> GetId (int id)
        {           
            var aux = await _es.GetEventoId(id);

            if(aux != null)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound($"Não foi possível localizar evento com id: {id}");
            }             
        }


        [HttpGet("GetNome")]
        public async Task<IActionResult> GetNome(string nome)
        {
           
            var aux = await _es.GetEventoNomes(nome);

            if( aux != null )
            {
                return Ok(aux);
            }
            else
            {
                return NotFound($"Não foi possível localizar o evento: {nome}");
            }                        
        }


        [HttpPost("CreateEvento")]
        [Authorize(Policy = "Super")]
        public async Task<IActionResult> CreateEventos(EventoDTO evento)
        {
            
            var aux = await _es.CreateEvento(evento);

            if(aux is true)
            {
                return Ok(aux);                                               
            }
            else
            {
                return BadRequest($"Evento com nome: '{_es.NormalizeNome(evento.Nome)}' já existe");
            }            
        }


        [HttpPatch("UpdateADM")]
        [Authorize(Policy = "Super")]
        public async Task<IActionResult> UpdateADM(int id, CreateEventoDTO dto)
        {
           
            var aux = await _es.AtualizarEventoADM(id, dto);

            if (aux is true)
            {
                return Ok(aux);
            }
            else
            {
                return BadRequest("Não foi possível atualizar evento ADM");
            }                      
        }


        [HttpPatch("Update")]
        [Authorize(Policy = "Super")]
        public async Task<IActionResult> UpdateEvento(int id, EventoDTO dto)
        {
            
            var aux = await _es.AtualizarEvento(id, dto);

            if(aux is true)
            {
                return Ok(aux);
            }
            else
            {
                return BadRequest("Não foi possível atualizar evento");

            }
        }



        [HttpDelete("Delete")]
        [Authorize(Policy = "Super")]
        public async Task<IActionResult> DeleteEvento(int id)
        {  
            
            var aux = await _es.DeletarEventos(id);

            if (aux is true)
            {
                return Ok(aux);
            }
            else
            {
                return NotFound("Não foi possível deletar o evento");
            }                        
        }



        [HttpPatch("Inscricao/{id}")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> FazerInscricao(int id, InscricaoDTO dto)
        {
           
            var aux = await _es.Inscricao(id, dto);

            if (aux)
            {
                return Ok(aux);
            }
            else
            {
                return BadRequest($"Não foi possível realizar a inscrião no evento {dto.NomeEvento}");
            }
        }


        [HttpGet("EventosInscricoes")]
        [Authorize(Policy = "Super")]
        public async Task<ActionResult<ICollection<Evento>>> EventoInscricoes()
        {                       
            return Ok(await _es.EventoInscricao());                     
        }
    }
}
