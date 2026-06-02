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


        public async Task<ICollection<Inscricao>> GetAll()
        {
            return await _api.InscricaoRepository.GetAllAsync();
        }      
    }
}
