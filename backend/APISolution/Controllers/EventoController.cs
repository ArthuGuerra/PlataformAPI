using Application.DataTransferObject;
using Application.Interfaces;
using Domain.Entities;
using Infraestrutura.ContextRepository;
using Infraestrutura.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace APISolution.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            try
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
            } catch (Exception ex)
            {
                throw new Exception("falha ao encontrar a coleção de eventos");
            }
        }




        [HttpGet("{id}")]
        public async Task<ActionResult<EventoDTO>> GetId (int id)
        {
            try
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

                
            }catch (Exception ex)
            {
                throw new Exception($"Evento {id} não encontrado");
            }

        }


        [HttpGet("GetNome")]
        public async Task<ActionResult<EventoDTO>> GetNome(string nome)
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("Falha ao buscar o nome do evento");
            }
        }


        [HttpPost("CreateEvento")]
        public async Task<ActionResult<EventoDTO>> CreateEventos(EventoDTO evento)
        {
            try
            {
                var aux = await _es.CreateEvento(evento);

                if(aux != null)
                {
                    return Created();                                               
                }
                else
                {
                    return BadRequest($"Evento com nome: '{_es.NormalizeNome(evento.Nome)}' já existe");
                }


            } catch(Exception ex)
            {
                return BadRequest(ex.ToString());
            }
        }


        [HttpPatch("UpdateADM")]
        public async Task<ActionResult<EventoDTO>> UpdateADM(int id, CreateEventoDTO dto)
        {
            try
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
            catch(Exception ex)
            {
                throw new Exception("falha ao atualizar evento ADM");
            }
        }


        [HttpPatch("Update")]
        public async Task<ActionResult<EventoDTO>> UpdateEvento(int id, EventoDTO dto)
        {
            try
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
            catch(Exception)
            {
                throw new Exception("falha ao atualizar evento");

            }
        }



        [HttpDelete("Delete")]
        public async Task<ActionResult<EventoDTO>> DeleteEvento(int id)
        {
            try
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
            catch (Exception)
            {
                throw new Exception("falha ao deletar evento");

            }
        }



        [HttpPatch("Inscricao")]
        public async Task<ActionResult<EventoDTO>> FazerInscricao(InscricaoDTO dto, string nome)
        {
            try
            {
                var aux = await _es.Inscrição(dto,nome);

                if (aux != null)
                {
                    return Ok(aux);
                }
                else
                {
                    return BadRequest(aux);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
               

                    //new Exception("falha ao fazer inscrição no evento");

            }
        }


        [HttpGet("EventosInscricoes")]
        public async Task<ActionResult<ICollection<Evento>>> EventoInscricoes()
        {
            try
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
            catch (Exception ex)
            {
                throw new Exception("falha ao mostrar eventos e inscriçoes");
            }
        }





    }
}
