using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class Usuario
    {    
        public Usuario()
        {
            Inscricoes = new List<Inscricao>();
        }

        [Key]
        public int Id { get; set; }

        public string? Nome { get; set; }
        public string? Email { get; set; } 
        

        public ICollection<Inscricao> Inscricoes { get; set; }
    }
}
