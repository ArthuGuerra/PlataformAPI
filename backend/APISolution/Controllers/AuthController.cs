using Application.DataTransferObject.IdentityDTO;
using Application.Interfaces;
using Azure;
using Domain.Entities;
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
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _token;
        private readonly UserManager<Usuario> _user;
        private readonly RoleManager<IdentityRole> _role;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;



        public AuthController(ITokenService token, UserManager<Usuario> user, RoleManager<IdentityRole> role, IConfiguration config, ILogger<AuthController> logger)
        {
            _token = token;
            _user = user;
            _role = role;
            _config = config;
            _logger = logger;
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
        public async Task<IActionResult> AdduserRole(string email, string roleName)
        {
            var user = await _user.FindByEmailAsync(email);

            if(user != null)
            {
                var result = await _user.AddToRoleAsync(user, roleName);

                if(result.Succeeded)
                {
                    _logger.LogInformation(1, $"Usuário com {user.Email} foi adicionado a {roleName} role ");
                    return StatusCode(StatusCodes.Status201Created, $"Status: Success; Message: Usuário {user.Email} foi adicionado a {roleName} role");
                }
                else
                {
                    _logger.LogInformation(2, $"Error: Não foi possível adicionar {user.Email} a {roleName} role");
                    return StatusCode(StatusCodes.Status400BadRequest, $"Status: Error; Message: Não foi possível adicionar {user.Email} a {roleName} role");
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, $"Status: Error; Message: Role {roleName} já existe");
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
                    Expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }


        [HttpPost("Cadastro")]
        public async Task<IActionResult> Register(RegisterModelDTO model, IUsuarioServices _userServices)
        {

            var userExist = await _userServices.GetAllUsers();


            var norma = _userServices.NormalizeNome(model.Username!);
            var email = _userServices.NormalizeNome(model.Email!);

            var existe = userExist.Any(x => _userServices.NormalizeNome(x.UserName!) == norma);

            var existeEmail = userExist.Any(x => _userServices.NormalizeNome(x.Email!) == email);

            var existeCPF = userExist.Any(x => _userServices.NormalizeNome(x.CPF!) == model.CPF);


            if (existe || existeEmail || existeCPF)
            {
                return BadRequest("Usuario, Email ou CPF existente!");
            }
            else
            {
                Usuario user = new()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.Username,
                    CPF = model.CPF,
                    PhoneNumber = model.Telefone,                 
                };

                var result = await _user.CreateAsync(user, model.Password!);

                if (!result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
                else
                {
                    return Ok(result);
                }
            }

            
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

            string username = principal.Identity.Name;

            var user = await _user.FindByNameAsync(username!);

            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return BadRequest("Invalid access/Refresh token");
            }

            var newAccessToken = _token.GenerateAccessToken(principal.Claims.ToList(), _config);

            var newRefreshToken = _token.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
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
            var aux = await _user.Users.ToListAsync();

            if(aux == null)
            {
                return NotFound();
            }
            else
            {
                return aux;
            }
        }


        [HttpGet("ShowRoles")]
        [Authorize(Policy = "Super")]
        public async Task<ActionResult<ICollection<IdentityRole>>> ShowRoles()
        {
            var aux = await _role.Roles.ToListAsync();

            if (aux == null)
            {
                return NotFound();
            }
            else
            {
                return aux;
            }
        }

    }
}
