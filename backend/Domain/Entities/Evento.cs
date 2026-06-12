using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class Evento
    {

        public Evento()
        {
            Inscricoes = new List<Inscricao>();
        }

        [Key]
        public int Id { get; set; }
        public string? Nome { get; set; }
        public double Preco { get; set; }
        public string? Imagem { get; set; }
        public DateTime DataInscricao { get; set; }
        public DateTime DataEvento { get; set; }
        public string? Descricao { get; set; }
        public string? LocalEvento { get; set; }
        public int QuantidadeDeCamisasDisponiveis { get; set; } = 2000;

        public ICollection<Inscricao> Inscricoes { get; set; }

    }
}
