using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Inscricao
    {
        [Key]
        public int Id { get; set; }

        [Range(0,1)]
        public int QuantidadeKit { get; set; }

        public string? TamanhoCamisa { get; set; }

        public string? QuantidadeKm { get; set; }
        public DateTime DataDeInscricaoDousuario { get; set; }

        public string? UsuarioId { get; set; }
        public string? NomeEvento { get; set; }

        [JsonIgnore]
        public Usuario? Usuario {  get; set; }
        public int EventoId { get; set; }

        [JsonIgnore]
        public Evento? Evento { get; set; }
    }
}
