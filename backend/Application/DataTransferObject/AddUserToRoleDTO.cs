using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DataTransferObject
{
    public class AddUserToRoleDTO
    {
        public string? Email { get; set; }
        public string? RoleName { get; set; }
    }
}
