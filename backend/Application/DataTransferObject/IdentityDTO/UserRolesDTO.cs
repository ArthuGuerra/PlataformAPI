using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DataTransferObject.IdentityDTO
{
    public class UserRolesDTO
    {
        public string? UserName { get; set; }
        public IList<string>? Roles { get; set; }
    }
}
