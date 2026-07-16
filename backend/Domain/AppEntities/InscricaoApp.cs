using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace Domain.AppEntities
{
    public class InscricaoApp
    {
        [Key]
        public int Id { get; set; }


        public Guid UsuarioId { get; set; }  
        public Usuario? UsuarioApp { get; set; }
        public int TurmaId { get; set; }                
        public Turmas? Turma { get; set; }

        
    }
}
