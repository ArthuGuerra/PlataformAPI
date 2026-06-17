using Application.DataTransferObject;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Plataforma_Front.Controllers
{
    public class EventoController : Controller
    {

        private readonly HttpClient _client;

        public EventoController(IHttpClientFactory client)
        {
            _client = client.CreateClient();
        }


        // GET: EventoController
        public async Task<ActionResult> Index()
        {
            var eventos = await _client.GetFromJsonAsync<List<EventoDTO>>
                (
                   "https://localhost:7187/api/Evento/EventosDTO"
                );

            return View(eventos);
        }

        // GET: EventoController/Details/5

        public async Task<ActionResult> GetNome(string nome)
        {
            var evento = await _client.GetFromJsonAsync<EventoDTO>
             (
                 $"https://localhost:7187/Evento/GetNome?nome={nome}"
             );
            return View(evento);
        }

        // GET: EventoController/Create
        public async Task<ActionResult> FazerInscricao(InscricaoDTO dto,string nome)
        {
            var evento = await _client.GetFromJsonAsync<InscricaoDTO>
                (
                    $"https://localhost:7187/Evento/Inscricao?nome={nome}"
                );
            return View(evento);
        }

        // POST: EventoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EventoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: EventoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EventoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EventoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
