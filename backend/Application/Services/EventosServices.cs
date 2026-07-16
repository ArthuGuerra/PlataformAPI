
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
        private readonly IInscricaoServices _ins;


        public EventosServices(IUnitOfWork api, IMapper mapper, IInscricaoServices ins)
        {
            _api = api;
            _mapper = mapper;
            _ins = ins;  
        }


       
        public async Task<ICollection<EventoDTO>> ListarEventos()
        {    
            var aux = await _api.EventosRepository.GetAllAsync();

            return _mapper.Map<ICollection<EventoDTO>>(aux);
                           
        }


        public async Task<EventoDTO> GetEventoId(int id)
        {
            var aux = await _api.EventosRepository.GetIdAsync(id);

            return _mapper.Map<EventoDTO>(aux);        

        }


        public async Task<EventoDTO> GetEventoNomes(string nome)
        {
            var aux = await _api.EventosRepository.GetEventoNome(nome);

            return _mapper.Map<EventoDTO>(aux);
         
        }


        public async Task<string> CreateEvento(EventoDTO evento)
        {

            var ev = await _api.EventosRepository.GetAllAsync();

            var normalizado = NormalizeNome(evento.Nome);

            var existe = ev.Any(x => NormalizeNome(x.Nome) == normalizado);

            if(existe)
            {
                return null;
            }
            else
            {
                var map = _mapper.Map<Evento>(evento);

                _api.EventosRepository.Create(map);

                await _api.Commit();

                return $"Evento: '{evento.Nome}' criado.";
            }                                      
        }



        public async Task<string> AtualizarEventoADM(int id, CreateEventoDTO dto) 
        {
            var aux = await _api.EventosRepository.GetIdAsync(id); 
            
            if(aux != null)
            {
                aux.Preco = dto.Preco;
                aux.QuantidadeDeKitsDisponiveis = dto.QuantidadeDeKitsDisponiveis;


                _api.EventosRepository.Update(aux);

                await _api.Commit();

                return "Atualização do evento concluída";
            }
            else
            {
                return $"Evento {id} nao encontrado";
            }
        }



        

        public async Task<EventoDTO> AtualizarEvento(int id, EventoDTO dto)
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
            }
            

            _api.EventosRepository.Update(aux);

            await _api.Commit();

            return _mapper.Map<EventoDTO>(aux);
        }     




        public async Task<EventoDTO> DeletarEventos(int id)
        {
            var aux = await _api.EventosRepository.GetIdAsync(id);
         
            if(aux != null)
            {
                _api.EventosRepository.Delete(aux);
                await _api.Commit();

                return _mapper.Map<EventoDTO>(aux);
            }
            else
            {
                return null;
            }
                   

        }




        public async Task<InscricaoDTO> Inscrição(InscricaoDTO dto)
        {
            var aux = await _api.EventosRepository.GetIdAsync(dto.EventoId!);


            if (aux == null)
            {
                return null;
            }
            
            var inscricao = _mapper.Map<Inscricao>(dto);                       
           
            var inscricoes = await _ins.GetAll();

            var existe = inscricoes.Any(x => 
            x.EventoId ==  dto.EventoId && 
            x.UsuarioId == dto.UsuarioId);            

            if(existe)
            {
                return null;
            }
            else
            {
               
                if(dto.QuantidadeKit == 1)
                {
                   aux.QuantidadeDeKitsDisponiveis--;
                }
                else
                {
                    dto.TamanhoCamisa = null;                                      
                }
                
                inscricao.DataDeInscricaoDousuario = DateTime.UtcNow;

                _api.InscricaoRepository.Create(inscricao);

                await _api.Commit();

                return _mapper.Map<InscricaoDTO>(inscricao);
            }                                 
        }


        public string NormalizeNome(string nome)
        {                  
            return string.Join(" ", nome
                .Trim()
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));       
        }

        public async Task<ICollection<Evento>> EventoInscricao()
        {
            var aux = await _api.EventosRepository.GetEventoInscricao();

            if(aux == null)
            {
                return null;
            }
            else
            {
                return aux;
            }
        }
    }
}
