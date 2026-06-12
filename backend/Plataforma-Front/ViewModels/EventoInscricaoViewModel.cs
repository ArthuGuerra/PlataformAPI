using Application.DataTransferObject;
using Application.Interfaces;

namespace Plataforma_Front.ViewModels
{
    public class EventoInscricaoViewModel
    {
        private IEventoServices _services;

        public EventoInscricaoViewModel(IEventoServices services)
        {
            _services = services;
        }

        public EventoDTO Dto { get; set; }
        
    }
}
