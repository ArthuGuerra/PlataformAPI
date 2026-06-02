using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DataTransferObject
{
    public class UsuarioCorredorDTO
    {


        [Required(ErrorMessage = "Nome obrigatório")]
        public string? Nome { get; set; }

        [Required(ErrorMessage ="E-mail obrigatório")]
        [EmailAddress(ErrorMessage ="formato de e-mail inválido")]
        public string? Email { get; set; }
    }
}
