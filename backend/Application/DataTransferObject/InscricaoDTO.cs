using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.DataTransferObject
{
    public class InscricaoDTO
    {

        public string? UsuarioId { get; set; }
        public int EventoId { get; set; }
        public string? NomeEvento { get; set; }
        public int Id { get; set; }

        
        [Range(0,1)]
        public int Camisa { get; set; }
        public string? Cor { get; set; }
        public char? TamanhoCamisa { get; set; }
    }
}
