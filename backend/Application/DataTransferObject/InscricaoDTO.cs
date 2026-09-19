using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.DataTransferObject
{
    public class InscricaoDTO
    {

        public int Id { get; set; }
        public string? UsuarioId { get; set; }
        public int EventoId { get; set; }
        public string? NomeEvento { get; set; }   

                
        public string? QuantidadeKm { get; set; }
        public string? TamanhoCamisa { get; set; }
        public string? Genero { get; set; }
        public DateTime DataDeInscricao { get; set; }
    }
}
