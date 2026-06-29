using Application.DataTransferObject;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Infraestrutura.Interfaces;
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

            return _mapper.Map<ICollection<InscricaoDTO>>(aux);               

        }


        public async Task<InscricaoDTO> Delete(int id)
        {
            var aux = await _api.InscricaoRepository.GetIdAsync(id);            
                        
             _api.InscricaoRepository.Delete(aux);

            await _api.Commit();

            return _mapper.Map<InscricaoDTO>(aux);
        }
    }
}
