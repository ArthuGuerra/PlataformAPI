using Application.DataTransferObject.IdentityDTO;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        JwtSecurityToken GenerateAccessToken(
          IEnumerable<Claim> claims);

        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(
            string token);

    }
}
