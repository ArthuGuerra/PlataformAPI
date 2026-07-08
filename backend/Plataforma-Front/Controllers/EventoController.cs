using Application.DataTransferObject;
using AspNetCoreGeneratedDocument;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Front.Interfaces;
using Plataforma_Front.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Plataforma_Front.Controllers
{
    public class EventoController : Controller
    {
        private readonly IEventosServicesMVC _evento;
        private string token = string.Empty;

        public EventoController(IEventosServicesMVC evento)
        {
            _evento = evento;
        }


        private string ObterTokenJWT()
        {
            if (HttpContext.Request.Cookies.ContainsKey("X-Access-Token"))
                token = HttpContext.Request.Cookies["X-Access-Token"].ToString();

            return token;
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
            if(nome == null)
            {
                return View("Error");
            }

            var evento = await _evento.GetEventoNome(nome);
            if(evento == null)
            {
                ModelState.AddModelError("", "Evento não encontrado");
                return View("Error",new EventoInscricaoViewModel());
                // pensar em retirar isso aqui
            }



            var vm = new EventoInscricaoViewModel
            {
                Evento = evento,
                Inscricao = new InscricaoDTO(),
                User = new Usuario()
            };

                return View("GetNome", vm);
        }

        

        [HttpPost]
        public async Task<IActionResult> FazerInscricaoPOST(EventoInscricaoViewModel model)
        {

            var token = Request.Cookies["X-Access-Token"];

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);          

            var userId = jwt.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;


            if (model == null && userId == null)
            {
                ViewBag.Erro = "Dados para inscrição inválidos ou Usuário não Autenticado.";
                return View("Error");
            }
            else
            {

                var dto = new InscricaoDTO
                {
                    UsuarioId = userId,
                    EventoId = model.Evento.Id,
                    NomeEvento = model.Evento.Nome,
                    QuantidadeKit = model.Inscricao.QuantidadeKit,
                    QuantidadeKm = model.Inscricao.QuantidadeKm,
                    TamanhoCamisa = model.Inscricao.TamanhoCamisa
                };

                await _evento.FazerInscricaoService(dto, ObterTokenJWT());

                return RedirectToAction("Index", "Inscricao");
            }
        

        }




    }
}

