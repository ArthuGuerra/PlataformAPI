using Application.DataTransferObject.IdentityDTO;
using Application.Interfaces;
using Asp.Versioning;
using Azure;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APISolution.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly UserManager<Usuario> _user;
        private readonly RoleManager<IdentityRole> _role;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;
        private IUsuarioServices _userServices;



        public AuthController(ITokenService token, UserManager<Usuario> user, RoleManager<IdentityRole> role, IConfiguration config, ILogger<AuthController> logger, IUsuarioServices userServices)
        {
            _token = token;
            _user = user;
            _role = role;
            _config = config;
            _logger = logger;
            _userServices = userServices;
        }

        [Authorize(Policy = "Super")]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var roleExist = await _role.RoleExistsAsync(roleName);

            if (!roleExist)
            {
                var roleResult = await _role.CreateAsync(new IdentityRole(roleName));

                if (roleResult.Succeeded)
                {
                    _logger.LogInformation(1, "Roles Added");
                    return StatusCode(StatusCodes.Status201Created, $"Status: Success; Message: Role {roleName} added successfully");
                }
                else
                {
                    _logger.LogInformation(2, "Error");
                    return StatusCode(StatusCodes.Status400BadRequest, $"Status: Error; Message: Issue adding the new {roleName} role");
                }
            }
            return StatusCode(StatusCodes.Status400BadRequest, $"Status: Error; Message: Role {roleName} já existe");
        }


        [Authorize(Policy = "Super")]
        [HttpPost("AddUserToRole")]
        public async Task<IActionResult> AddUserRole(string email, string roleName)
        {
            var user = await _user.FindByEmailAsync(email);

            if (user != null)
            {
                var exist = await _user.IsInRoleAsync(user, roleName);

                if (exist)
                {
                    return BadRequest($"Usuario: {user.Email} já possui a role {roleName}");
                }
                else
                {
                    var result = await _user.AddToRoleAsync(user, roleName);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation(1, $"Usuário: {user.Email} foi adicionado a {roleName} role");

                        return StatusCode(StatusCodes.Status201Created, $"Status: Success; Message: Usuário {user.Email} foi adicionado a {user.Email} foi adicionado a {roleName} role");
                    }
                    else
                    {
                        _logger.LogInformation(2, $"Error: Não foi possível adicionar {user.Email} a {roleName} role");
                        return StatusCode(StatusCodes.Status400BadRequest, $"Status: Error; Message: Não foi possível adicionar {user.Email} a {roleName} role");
                    }
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, $"Status: Error; Message: {user.Email} não foi encontrado ou nao existe.");
            }
        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModelDTO model)
        {
            var user = await _user.FindByNameAsync(model.Username!);


            if (user is not null && await _user.CheckPasswordAsync(user, model.Password!))
            {
                var userRoles = await _user.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim("userId", user.Id!),          
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };               

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var token = _token.GenerateAccessToken(authClaims, _config);

                var refreshToken = _token.GenerateRefreshToken();

                _ = int.TryParse(_config["JWT:RefreshTokenValidityInHours"], out int refreshTokenValidityInHours);

                user.RefreshToken = refreshToken;

                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(refreshTokenValidityInHours);

                await _user.UpdateAsync(user);

                return Ok(new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken,
                    Expiration = token.ValidTo,
                    Authenticated = true,
                    Message = "Usuario autenticado com sucesso"
                });
            }

            return Unauthorized();
        }


        [HttpPost("Cadastro")]
        public async Task<IActionResult> Register(RegisterModelDTO model)
        {
            var userExist = await _userServices.GetAllUsers();

            var norma = _userServices.NormalizeNome(model.Username!);
            var email = _userServices.NormalizeNome(model.Email!);

            var existe = userExist.Any(x =>
                _userServices.NormalizeNome(x.UserName!) == norma);

            var existeEmail = userExist.Any(x =>
                _userServices.NormalizeNome(x.Email!) == email);

            var existeCPF = userExist.Any(x =>
                _userServices.NormalizeNome(x.CPF) == model.CPF);

            if (existe || existeEmail || existeCPF)
            {
                return BadRequest("Usuario, Email ou CPF existente!");
            }

            var user = new Usuario
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                CPF = model.CPF,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _user.CreateAsync(user, model.Password!);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var roleResult = await _user.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                // Evita deixar um usuário criado sem a role esperada
                await _user.DeleteAsync(user);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(result);
        }



        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenModelDTO model)
        {
            if (model is null)
            {
                return BadRequest("Request de cliente inválido");
            }

            string? accessToken = model.AccessToken ?? throw new ArgumentNullException(nameof(model));

            string? refreshToken = model.RefreshToken ?? throw new ArgumentNullException(nameof(model));

            var principal = _token.GetPrincipalFromExpiredToken(accessToken!, _config);

            if (principal == null)
            {
                return BadRequest("Access/Refresh Token inválido");
            }

            string username = principal.Identity?.Name;

            if(string.IsNullOrEmpty(username))
            {
                return BadRequest("Token Inválido");
            }

            var user = await _user.FindByNameAsync(username!);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return BadRequest("Invalid access/Refresh token");
            }

            var newAccessToken = _token.GenerateAccessToken(principal.Claims.ToList(), _config);

            var newRefreshToken = _token.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(3);
            await _user.UpdateAsync(user);

            return new ObjectResult(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                refreshToken = newRefreshToken
            });
        }

 
        [HttpPost("revoke/{username}")]
        [Authorize(Policy = "Super")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _user.FindByNameAsync(username);

            if (user == null) return BadRequest("Invalid user name");

            user.RefreshToken = null;

            await _user.UpdateAsync(user);

            return NoContent();
        }


        [HttpGet("ShowUsers")]
        [Authorize(Policy = "Super")]
        public async Task<ActionResult<ICollection<Usuario>>> ShowUsers()
        {
            return Ok(await _user.Users.ToListAsync());

        }


        [HttpGet("ShowRoles")]
        [Authorize(Policy = "Super")]
        public async Task<ActionResult<ICollection<IdentityRole>>> ShowRoles()
        {
            return Ok(await _role.Roles.ToListAsync());            
        }

    }
}
