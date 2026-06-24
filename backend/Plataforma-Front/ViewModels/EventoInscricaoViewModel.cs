using Application.DataTransferObject;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Plataforma_Front.ViewModels
{
    public class EventoInscricaoViewModel
    {
                
        public Usuario? User { get; set; }
        public InscricaoDTO? Inscricao { get; set; }
        public EventoDTO? Evento { get; set; }
    }
}
