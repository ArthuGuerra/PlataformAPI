using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DataTransferObject
{
    public class EventoDTO
    {
        
        public string? Nome { get; set; }
        public string? Imagem { get; set; }
        public DateTime DataInscricao { get; set; }
        public DateTime DataEvento { get; set; }
        public string? Descricao { get; set; }     

    }
}
