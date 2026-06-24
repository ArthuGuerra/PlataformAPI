using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Application.DataTransferObject
{
    public class UsuarioPrintDTO
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        
        
        [JsonIgnore]
        public string? CPF { get; set; }
    }
}
