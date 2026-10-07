using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DataTransferObject
{
    public class ChangePassDTO
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Usuario obrigatório")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Senha atual obrigatória")]
        public string? SenhaAtual { get; set; }

        [Required(ErrorMessage = "Senha é obrigatório")]
        public string? NewSenha { get; set; }
    }
}
