using Application.DataTransferObject;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infraestrutura.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class InscricaoServices : IInscricaoServices
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _api;
 

        public InscricaoServices(IMapper mapper, IUnitOfWork api)
        {
            _mapper = mapper;
            _api = api;            
        }


        public async Task<ICollection<InscricaoDTO>> GetAll()
        {
            var aux = await _api.InscricaoRepository.GetAllAsync();

            if(aux.Count == 0)
            {
                return [];
            }
            else
            {               
                return _mapper.Map<ICollection<InscricaoDTO>>(aux);
            }

        }


        
        public async Task<bool> Delete(int id)
        {
            var aux = await _api.InscricaoRepository.GetIdAsync(id);

            if(aux != null)
            {

                var ev = await _api.EventosRepository.GetIdAsync(aux.EventoId);

                ev.QuantidadeDeKitsDisponiveis++;

                _api.InscricaoRepository.Delete(aux);

                await _api.Commit();
                
                return true;
            }
            else
            {                
                return false;
            }

        }




    }
}
