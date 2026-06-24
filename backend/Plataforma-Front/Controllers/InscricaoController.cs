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

        public async Task<IActionResult> Index()
        {
            var aux = await _ins.ShowMyIncricoes();          

            return View(aux);
        }
    }
}
