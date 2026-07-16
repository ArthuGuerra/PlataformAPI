using System;
using System.Collections.Generic;
using System.Text;
using Domain.AppEntities;
using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infraestrutura.BancoContexto
{
    public class ApiContext : IdentityDbContext<Usuario>
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }



        public DbSet<Usuario>? Usuario { get; set; }
        public DbSet<Evento>? Evento { get; set; }
        public DbSet<Inscricao> Inscricao { get; set; }
        public DbSet<InscricaoApp> InscricoesAulas { get; set; }
        public DbSet<Turmas> Turmas { get; set; }
        


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}