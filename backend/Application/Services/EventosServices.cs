
using Application.DataTransferObject;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infraestrutura.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Application.Services
{
    public class EventosServices : IEventoServices
    {
        private readonly IUnitOfWork _api;
        private readonly IMapper _mapper;
        


        public EventosServices(IUnitOfWork api, IMapper mapper)
        {
            _api = api;
            _mapper = mapper;             
        }


       
        public async Task<ICollection<EventoDTO>> ListarEventos()
        {    
            var aux = await _api.EventosRepository.GetAllAsync();

            if (aux.Count == 0)
            {
                return [];
            }
            else
            {
                return _mapper.Map<ICollection<EventoDTO>>(aux);
            }
        }

        public async Task<ICollection<EventoDTO>> EventosAtivos()
        {
            var eventos = await _api.EventosRepository.GetEventosAtivos();

            if(eventos.Count == 0)
            {
                return [];
            }
            else
            {
                return _mapper.Map<ICollection<EventoDTO>>(eventos);
            }
        }
            



        public async Task<EventoDTO> GetEventoId(int id)
        {
            var aux = await _api.EventosRepository.GetIdAsync(id);

            if(aux != null)
            {
                return _mapper.Map<EventoDTO>(aux);
            }
            else
            {
                return null;
            }
        }


        public async Task<EventoDTO> GetEventoNomes(string nome)
        {
            var aux = await _api.EventosRepository.GetEventoNome(nome);

            if(aux != null)
            {
                return _mapper.Map<EventoDTO>(aux);
            }
            else
            {
                return null;
            }


        }


        public async Task<bool> CreateEvento(EventoDTO evento)
        {

            var ev = await _api.EventosRepository.GetAllAsync();

            var normalizado = NormalizeNome(evento.Nome);

            var existe = ev.Any(x => NormalizeNome(x.Nome) == normalizado);

            if(existe)
            {
                return false;
            }
            else
            {
                var map = _mapper.Map<Evento>(evento);

                _api.EventosRepository.Create(map);

                await _api.Commit();

                return true;
            }                                      
        }



        public async Task<bool> AtualizarEventoADM(int id, CreateEventoDTO dto) 
        {
            return await _api.EventosRepository.UpdateADM(id,dto.Preco,dto.QuantidadeDeKitsDisponiveis);
        }

        

        public async Task<bool> AtualizarEvento(int id, EventoDTO dto)
        {

            var aux = await _api.EventosRepository.GetIdAsync(id);

            if(aux != null)
            {
                aux.Descricao = dto.Descricao;
                aux.Imagem = dto.Imagem;
                aux.DataEvento = dto.DataEvento;
                aux.Descricao = dto.Descricao;
                aux.Nome = dto.Nome;
                aux.LocalEvento = dto.LocalEvento;

                _api.EventosRepository.Update(aux);

                await _api.Commit();

                return true;
            }
            else
            {
                return false;

            }


        }     




        public async Task<bool> DeletarEventos(int id)
        {
            var aux = await _api.EventosRepository.GetIdAsync(id);
         
            if(aux != null)
            {
                //_api.EventosRepository.Delete(aux);

                aux.Ativo = false;

                _api.EventosRepository.Update(aux);

                await _api.Commit();

                return true;
            }
            else
            {
                return false;
            }
                   

        }




        public async Task<bool> Inscricao(int id, InscricaoDTO dto)
        {

            var map = _mapper.Map<Inscricao>(dto);

            return await _api.EventosRepository.FazerInscricao(id, map);
                                                      
        }


        public async Task<ICollection<Evento>> EventoInscricao()
        {
            var aux = await _api.EventosRepository.GetEventoInscricao();

            if (aux.Count == 0)
            {
                return [];
            }
            else
            {
                return aux;
            }
        }



        public string NormalizeNome(string nome)
        {                  
            return string.Join(" ", nome
                .Trim()
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));       
        }
        
    }
}
