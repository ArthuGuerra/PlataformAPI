using Application.DataTransferObject;
using AspNetCoreGeneratedDocument;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Front.Interfaces;
using Plataforma_Front.ViewModels;
using System.ComponentModel.DataAnnotations;
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
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult>FazerInscricaoPOST(int id, EventoInscricaoViewModel model)
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
                    QuantidadeKm = model.Inscricao.QuantidadeKm,
                    TamanhoCamisa = model.Inscricao.TamanhoCamisa,
                    Genero = model.Inscricao.Genero
                    
                };

                await _evento.FazerInscricaoService(id, dto, ObterTokenJWT());

                return RedirectToAction("Index", "Inscricao");
            }        
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]        
        public async Task<ActionResult> CreateEve()
        {
            return View(new EventoDTO());
        }



        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventoDTO evento)
        {
            var token = Request.Cookies["X-Access-Token"];

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var userId = jwt.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;


            if (evento == null && userId == null)
            {
                ViewBag.Erro = "Dados para inscrição inválidos ou Usuário não Autenticado.";
                return View("Error");
            }
            else
            {
                var dto = new EventoDTO
                {
                    Nome = evento.Nome,
                    Imagem = evento.Imagem,
                    DataEvento = evento.DataEvento,
                    Descricao = evento.Descricao,
                    LocalEvento = evento.LocalEvento                    
                };

                await _evento.CriarEvento(dto, ObterTokenJWT());

                return RedirectToAction("Index", "Evento");
            }
        }

      

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateADM(Evento evento)
        {
            var token = Request.Cookies["X-Access-Token"];

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var userId = jwt.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;


            if (userId == null)
            {
                ViewBag.Erro = "Dados para inscrição inválidos ou Usuário não Autenticado.";
                return View("Error");
            }
            else
            {
                var dto = new Evento
                {
                    Id = evento.Id,
                    QuantidadeDeKitsDisponiveis = evento.QuantidadeDeKitsDisponiveis,
                    Preco = evento.Preco
                };

                await _evento.UpdateEventoADM(dto, ObterTokenJWT());

                return RedirectToAction("Index", "Evento");
            }
        }       


        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEE(EventoDTO evento)
        {
            var token = Request.Cookies["X-Access-Token"];

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var userId = jwt.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;


            if (evento == null && userId == null)
            {
                ViewBag.Erro = "Dados para inscrição inválidos ou Usuário não Autenticado.";
                return View("Error");
            }
            else
            {
               
                await _evento.UpdateEvento(evento, ObterTokenJWT());

                return RedirectToAction("Index", "Evento");
            }
        }
     

        [HttpPost]
        [Authorize(Roles ="SuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEE(int id)
        {
            var token = Request.Cookies["X-Access-Token"];

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            var userId = jwt.Claims
                .FirstOrDefault(x => x.Type == "userId")?.Value;


            if (id <= 0 && userId == null)
            {
                ViewBag.Erro = "Dados para inscrição inválidos ou Usuário não Autenticado.";
                return View("Error");
            }
            else
            {                
                await _evento.DeleteEvento(id, ObterTokenJWT());
                return RedirectToAction("Index", "Evento");
            }
        }






    }
}

