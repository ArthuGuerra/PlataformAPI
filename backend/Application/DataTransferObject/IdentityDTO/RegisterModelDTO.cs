using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DataTransferObject.IdentityDTO
{
    public class RegisterModelDTO
    {
        [Required(ErrorMessage = " User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = " E-mail is required")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = " Password is required")]
        public string? Password { get; set; }
    }
}
