using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.AppEntities
{
    public class Turmas
    {
        public Turmas()
        {
            Inscricoes = new List<InscricaoApp>();
        }

        [Key]
        public int Id { get; set; }

        public string? NomeProfessor { get; set; }
        public DateTime HorarioDeInicioDaAula { get; set; }
        public DateTime HorarioDeTerminoDaAula { get; set; }
        public Esportes Esporte { get; set; }
        public int QuantidadeDeAlunos { get; set; }
        public string? LocalDaAula { get; set; }
        public ICollection<InscricaoApp>? Inscricoes { get; set; }
    }
}
