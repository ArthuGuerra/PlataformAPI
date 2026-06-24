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

        public InscricaoController(IInscricaoServiceMVC ins)
        {
            _ins = ins;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var aux = await _ins.ShowMyIncricoes();
            
            if(aux == null)
            {
                return View("Error");
            }

            return View(aux);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteInscricao(int id)
        {
            var aux = await _ins.DeleteMyIncricao(id);

            if(aux == null)
            {
                return View("Error");
            }
            return RedirectToAction("Index");
        }
    }
}
