using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class Usuario : IdentityUser
    {    
        public Usuario()
        {
            Inscricoes = new List<Inscricao>();
        }

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public ICollection<Inscricao> Inscricoes { get; set; }
    }
}
