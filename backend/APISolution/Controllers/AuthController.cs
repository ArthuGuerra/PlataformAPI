using Application.Configuration;
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
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
        private readonly ILogger<AuthController> _logger;
        private IUsuarioServices _userServices;
        private readonly JwtOptions _jwtOptions;
        private readonly SignInManager<Usuario> _signInManager;



        public AuthController(ITokenService token, UserManager<Usuario> user, RoleManager<IdentityRole> role, ILogger<AuthController> logger, IUsuarioServices userServices, IOptions<JwtOptions> jwtOptions, SignInManager<Usuario> signInManager)
        {
            _token = token;
            _user = user;
            _role = role;
            _logger = logger;
            _userServices = userServices;
            _jwtOptions = jwtOptions.Value;
            _signInManager = signInManager;
        }

        private static string HashRefreshToken(string refreshToken)
        {
            return Convert.ToBase64String(
                SHA256.HashData(
                    Encoding.UTF8.GetBytes(refreshToken)));
        }




        [Authorize(Policy = "Super")]
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            var roleExists = await _role.RoleExistsAsync(roleName);

            if (roleExists)
            {
                return BadRequest($"Status: Error; Message: Role {roleName} já existe");
            }

            var roleResult = await _role.CreateAsync(new IdentityRole(roleName));

            if (roleResult.Succeeded)
            {
                _logger.LogInformation("Role {RoleName} adicionada", roleName);

                return StatusCode(StatusCodes.Status201Created,
                    $"Status: Success; Message: Role {roleName} added successfully");
            }

            _logger.LogError("Falha ao adicionar a role {RoleName}", roleName);

            return BadRequest( $"Status: Error; Message: Issue adding the new {roleName} role");
        }




        [Authorize(Policy = "Super")]
        [HttpPost("AddUserToRole")]
        public async Task<IActionResult> AddUserRole(string email, string roleName)
        {
            var user = await _user.FindByEmailAsync(email);

            if (user != null)
            {

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(roleName))
                {
                    return BadRequest("E-mail e nome da role são obrigatórios.");
                }


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
                return BadRequest("Usuário não encontrado.");
            }
        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModelDTO model)
        {
            var user = await _user.FindByNameAsync(model.Username!);

            if (user is null || !user.Ativo)
            {
                return Unauthorized();
            }

            var passwordResult = await _signInManager.CheckPasswordSignInAsync(
                         user,
                         model.Password!,
                         lockoutOnFailure: true);

            if (passwordResult.IsLockedOut)
            {
                _logger.LogWarning("Usuário bloqueado após tentativas inválidas: {UserId}", user.Id);

                return Unauthorized("Usuário temporariamente bloqueado.");
            }

            if (!passwordResult.Succeeded)
            {
                return Unauthorized("Usuário ou senha inválidos.");
            }

            var userRoles = await _user.GetRolesAsync(user);            

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("userId", user.Id!),
                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(
                    new Claim(ClaimTypes.Role, userRole));
            }

            var token = _token.GenerateAccessToken(authClaims);

            var refreshToken = _token.GenerateRefreshToken();

            user.RefreshToken = HashRefreshToken(refreshToken);

            user.RefreshTokenExpiryTime =
                DateTime.UtcNow.AddHours(
                    _jwtOptions.RefreshTokenValidityInHours);

            var updateResult = await _user.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Não foi possível atualizar o usuário.");
            }

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler()
                    .WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo,
                Authenticated = true,
                Message = "Usuário autenticado com sucesso"
            });
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

            if (string.IsNullOrWhiteSpace(model.AccessToken) ||
                string.IsNullOrWhiteSpace(model.RefreshToken))
            {
                return BadRequest("Access token e refresh token são obrigatórios.");
            }


            var principal = _token.GetPrincipalFromExpiredToken(accessToken!);

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

            var refreshTokenHash = HashRefreshToken(refreshToken);

            if (user == null ||
                user.RefreshToken != refreshTokenHash ||
                user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return BadRequest("Access/Refresh token inválido");
            }

            var newAccessToken = _token.GenerateAccessToken(principal.Claims.ToList());

            var newRefreshToken = _token.GenerateRefreshToken();

            user.RefreshToken = HashRefreshToken(newRefreshToken);

            user.RefreshTokenExpiryTime =  DateTime.UtcNow.AddHours(
                    _jwtOptions.RefreshTokenValidityInHours);


            await _user.UpdateAsync(user);

            return Ok(new
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
