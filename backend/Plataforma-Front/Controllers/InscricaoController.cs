using Application.DataTransferObject;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Plataforma_Front.Interfaces;
using Plataforma_Front.ViewModels;
using System.Security.Claims;
using System.Text.Json;

namespace Plataforma_Front.Controllers
{
    public class InscricaoController : Controller
    {
        private readonly IInscricaoServiceMVC _ins;
        private string token = string.Empty;

        public InscricaoController(IInscricaoServiceMVC ins)
        {
            _ins = ins;
        }

        private string ObterTokenJWT()
        {
            if (HttpContext.Request.Cookies.ContainsKey("X-Access-Token"))
                token = HttpContext.Request.Cookies["X-Access-Token"].ToString();

            return token;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            
            var aux = await _ins.ShowMyIncricoes(ObterTokenJWT());
            
            if(aux == null)
            {
                return View("Error");
            }

            return View(aux);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteInscricao(int id)
        {
            var aux = await _ins.DeleteMyIncricao(id, ObterTokenJWT());

            if(aux == null)
            {
                return View("Error");
            }
            return RedirectToAction("Index");
        }
    }
}
