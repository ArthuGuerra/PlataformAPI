using Application.DataTransferObject;
using Application.DataTransferObject.AppDTO;
using AutoMapper;
using Domain.AppEntities;
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
            CreateMap<Usuario, UsuarioPrintDTO>().ReverseMap();
            CreateMap<UsuarioSenhaDTO, UsuarioPrintDTO>().ReverseMap();
            CreateMap<Inscricao,InscricaoDTO>().ReverseMap();

            CreateMap<InscricaoApp,InscricaoAppDTO>().ReverseMap();
            CreateMap<Turmas,TurmasDTO>().ReverseMap();
        }
    }
}
