using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DataTransferObject.IdentityDTO
{
    public class LoginModelDTO
    {
        [Required(ErrorMessage =" User Name is required")]
        public string? Username { get; set; }

        [Required(ErrorMessage = " Password is required")]
        public string? Password { get; set; }
    }
}
