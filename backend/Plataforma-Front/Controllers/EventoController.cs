using Application.DataTransferObject;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Front.Interfaces;
using Plataforma_Front.ViewModels;
using System.Security.Claims;

namespace Plataforma_Front.Controllers
{
    public class EventoController : Controller
    {
        private readonly IEventosServicesMVC _evento;

        public EventoController(IEventosServicesMVC evento)
        {
            _evento = evento;
        }


        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var eventos = await _evento.GetEventos();

            if (eventos == null)
            {
                return View("Error");
            }
            else
            {
                return View(eventos);
            }
        }


        [HttpGet]
        public async Task<ActionResult> GetNome()
        {           
            return View();
        }


        [HttpGet]
        public async Task<ActionResult> ProcurarNome(string nome)
        {            
                var evento = await _evento.GetEventoNome(nome);
            if(evento == null)
            {
                ModelState.AddModelError("", "Evento não encontrado");
                return View("Error",new EventoInscricaoViewModel());
            }
            var vm = new EventoInscricaoViewModel
            {
                Evento = evento,
                Inscricao = new InscricaoDTO()
            };

                return View("GetNome", vm);
        }

        

        [HttpPost]
        public async Task<IActionResult> FazerInscricaoPOST(EventoInscricaoViewModel model)
        {
            var claim = User.Claims.ToList();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        

            var dto = new InscricaoDTO 
            {
                UsuarioId = userId,
                EventoId = model.Evento.Id,
                Camisa = model.Inscricao.Camisa,
                Cor = model.Inscricao.Cor,
                TamanhoCamisa = model.Inscricao.TamanhoCamisa
            };

            await _evento.FazerInscricaoService(dto);

            return RedirectToAction("Index","Inscricao");
        }




    }
}

