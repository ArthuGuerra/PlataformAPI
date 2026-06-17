using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DataTransferObject.IdentityDTO
{
    public class TokenModelDTO
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
