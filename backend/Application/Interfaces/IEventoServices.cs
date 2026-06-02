using Application.DataTransferObject;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IEventoServices
    {
        public Task<string> CreateEvento(EventoDTO evento);
        public Task<EventoDTO> AtualizarEvento(int id, EventoDTO evento);
        public Task<string> AtualizarEventoADM(int id, CreateEventoDTO dto);
        public Task<ICollection<EventoDTO>> ListarEventos();
        public Task<EventoDTO> DeletarEventos(int id);
        public Task<EventoDTO> GetEventoId(int id);
        public Task<EventoDTO> GetEventoNomes(string nome);
        public Task<InscricaoDTO> Inscrição(InscricaoDTO dto, string nomeEvento);

        public Task<ICollection<Evento>> EventoInscricao();

        public string NormalizeNome(string nome);
    }
}
