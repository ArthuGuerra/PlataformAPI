using Domain.AppEntities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Domain.Entities
{
    [ExcludeFromCodeCoverage]
    public class Usuario : IdentityUser
    {    
        public Usuario()
        {
            Inscricoes = new List<Inscricao>();
            InscricoesApp = new List<InscricaoApp>();
        }
        
        public string? CPF { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        public ICollection<Inscricao> Inscricoes { get; set; }
        public ICollection<InscricaoApp> InscricoesApp { get; set; }
    }
}
