using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DataTransferObject
{
    public class InscricaoDTO
    {
        public string? UsuarioId { get; set; }
        public int EventoId { get; set; }
        public int Camisa { get; set; }
        public string? Cor { get; set; }
        public char? TamanhoCamisa { get; set; }
    }
}
