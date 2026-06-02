using Application.DataTransferObject;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.MapperExtension
{
    public class DomainDTOMappingProfile : Profile
    {
        public DomainDTOMappingProfile() 
        {
            CreateMap<Evento,EventoDTO>().ReverseMap();
            CreateMap<Evento,CreateEventoDTO>().ReverseMap();
            CreateMap<Usuario,UsuarioCorredorDTO>().ReverseMap();
            CreateMap<Inscricao,InscricaoDTO>().ReverseMap();
        }
    }
}
