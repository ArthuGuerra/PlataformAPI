using Application.DataTransferObject;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IEventoServices
    {
        public Task<bool> CreateEvento(EventoDTO evento);
        public Task<bool> AtualizarEvento(int id, EventoDTO evento);       
        public Task<ICollection<EventoDTO>> ListarEventos();
        public Task<ICollection<EventoDTO>> EventosAtivos();
        public Task<bool> DeletarEventos(int id);
        public Task<EventoDTO> GetEventoId(int id);


        public Task<bool> AtualizarEventoADM(int id, CreateEventoDTO dto);
        public Task<EventoDTO> GetEventoNomes(string nome);
        public Task<bool> Inscricao(int id, InscricaoDTO dto);
        public Task<ICollection<Evento>> EventoInscricao();

        public string NormalizeNome(string nome);
    }
}
