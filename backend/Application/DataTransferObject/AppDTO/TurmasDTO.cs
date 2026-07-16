using Domain.AppEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DataTransferObject.AppDTO
{
    public class TurmasDTO
    {
        public int Id { get; set; }
        public string? NomeProfessor { get; set; }
        public Esportes Esporte { get; set; }
        public DateTime HorarioDeInicioDaAula { get; set; }
        public DateTime HorarioDeTerminoDaAula { get; set; }      
        public int QuantidadeDeAlunos { get; set; }
        public string? LocalDaAula { get; set; }
    }
}
