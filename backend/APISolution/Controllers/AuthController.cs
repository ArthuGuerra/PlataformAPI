using Application.Configuration;
using Application.DataTransferObject;
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
using System.Text.RegularExpressions;

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

        private static string NormalizarCpf(string? cpf)
        {
            return Regex.Replace(cpf ?? string.Empty, @"\D", "");
        }




        [Authorize(Policy = "Super")]
        [HttpPost("CreateRole")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]    
        public async Task<IActionResult> AddUserRole(AddUserToRoleDTO? dto)
        {
            if (dto is null ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.RoleName))
            {
                return BadRequest("Falha ao completar essa operação");
            }

            var user = await _user.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                return BadRequest("Falha ao completar essa operação");
            }

            var jaPossuiRole = await _user.IsInRoleAsync(user, dto.RoleName);

            if (jaPossuiRole)
            {
                return BadRequest($"Usuário: {user.Email} já possui a role {dto.RoleName}");
            }

            var result = await _user.AddToRoleAsync(user, dto.RoleName);

            if (!result.Succeeded)
            {
                return BadRequest("Não foi possível adicionar a role ao usuário.");
            }

            _logger.LogInformation(
                "Usuário {Email} foi adicionado à role {RoleName}",
                user.Email,
                dto.RoleName);

            return StatusCode(
                StatusCodes.Status201Created,
                $"Usuário {user.Email} foi adicionado à role {dto.RoleName}.");
        }



        [HttpPost("Login")]
        [ResponseCache(NoStore = true,Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Login(LoginModelDTO? model)
        {
            if (model is null ||
                string.IsNullOrWhiteSpace(model.Username) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest("Usuário e senha são obrigatórios.");
            }

            var user = await _user.FindByNameAsync(_userServices.NormalizeNome(model.Username));

            if (user is null || !user.Ativo)
            {
                return Unauthorized("Usuário ou senha inválidos.");
            }

            var passwordResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                model.Password,
                lockoutOnFailure: false);

            if (passwordResult.IsLockedOut)
            {
                _logger.LogWarning(
                    "Usuário poderia ser bloqueado após tentativas inválidas: {UserId}",
                    user.Id);

                return Unauthorized("Usuário ou senha inválidos.");
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
                new Claim("userId", user.Id),
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
                _logger.LogError(
                    "Falha ao salvar o refresh token do usuário {UserId}: {Errors}",
                    user.Id,
                    string.Join(
                        "; ",
                        updateResult.Errors.Select(x => x.Description)));

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
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> Register(RegisterModelDTO model)
        {
            if (model is null ||
                string.IsNullOrWhiteSpace(model.Username) ||
                string.IsNullOrWhiteSpace(model.Email) ||
                string.IsNullOrWhiteSpace(model.Password))                
            {
                return BadRequest("Usuário, email e senha são obrigatórios.");
            }

            var userExist = await _userServices.GetAllUsers();

            var norma = _userServices.NormalizeNome(model.Username);
            var email = _userServices.NormalizeNome(model.Email);

            var existe = userExist.Any(x =>
                _userServices.NormalizeNome(x.UserName!) == norma);

            var existeEmail = userExist.Any(x =>
                _userServices.NormalizeNome(x.Email!) == email);

            var cpfInformado = !string.IsNullOrWhiteSpace(model.CPF);
            var cpfNormalizado = NormalizarCpf(model.CPF);

            if (cpfInformado && cpfNormalizado.Length != 11)
            {
                return BadRequest("CPF inválido.");
            }

            var existeCPF = cpfInformado && userExist.Any(usuario =>
                NormalizarCpf(usuario.CPF) == cpfNormalizado);

            if (existe || existeEmail || existeCPF)
            {
                return BadRequest("Não foi possível concluir o cadastro.");
            }

            var user = new Usuario
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username,
                CPF = cpfInformado ? cpfNormalizado : null,
                PhoneNumber = model.PhoneNumber
            };

            IdentityResult result;

            try
            {
                result = await _user.CreateAsync(user, model.Password);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning(ex,
                    "A criação do usuário falhou por conflito de unicidade.");

                return BadRequest("Não foi possível concluir o cadastro com os dados informados.");
            }

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.Select(e => new
                {
                    e.Code,
                    e.Description
                }));
            }

            var roleResult = await _user.AddToRoleAsync(user, "User");

            if (!roleResult.Succeeded)
            {
                await _user.DeleteAsync(user);

                _logger.LogError(
                    "Falha ao atribuir a role User ao novo usuário {UserId}: {Errors}",
                    user.Id,
                    string.Join("; ", roleResult.Errors.Select(e => e.Code)));

                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Não foi possível concluir o cadastro.");
            }

            return Ok(result);
        }



        [HttpPost("RefreshToken")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> RefreshToken(TokenModelDTO model)
        {
            if (model is null ||
                string.IsNullOrWhiteSpace(model.AccessToken) ||
                string.IsNullOrWhiteSpace(model.RefreshToken))
            {
                return BadRequest("Access token e refresh token são obrigatórios.");
            }

            ClaimsPrincipal? principal;

            try
            {
                principal = _token.GetPrincipalFromExpiredToken(model.AccessToken);
            }
            catch (Exception ex) when (
                ex is Microsoft.IdentityModel.Tokens.SecurityTokenException ||
                ex is ArgumentException)
            {
                return BadRequest("Access/Refresh token inválido");
            }

            var userId = principal?.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Access/Refresh token inválido");
            }

            var user = await _user.FindByIdAsync(userId);

            if (user is null ||
                !user.Ativo ||
                user.RefreshToken is null ||
                await _user.IsLockedOutAsync(user))
            {
                return BadRequest("Access/Refresh token inválido");
            }

            var receivedHash = HashRefreshToken(model.RefreshToken);

            var hashValido = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(user.RefreshToken),
                Encoding.UTF8.GetBytes(receivedHash));

            if (!hashValido || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return BadRequest("Access/Refresh token inválido");
            }

            // Claims refeitas a partir do banco (roles atuais), igual ao Login
            var userRoles = await _user.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("userId", user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var newAccessToken = _token.GenerateAccessToken(claims);
            var newRefreshToken = _token.GenerateRefreshToken();

            user.RefreshToken = HashRefreshToken(newRefreshToken);
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(
                _jwtOptions.RefreshTokenValidityInHours);

            var updateResult = await _user.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Não foi possível renovar a sessão.");
            }

            return Ok(new
            {
                accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),refreshToken = newRefreshToken
            });
        }


        [HttpPost("revoke/{username}")]
        [Authorize(Policy = "Super")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]     
        public async Task<IActionResult> Revoke(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Nome de usuário é obrigatório.");
            }

            var user = await _user.FindByNameAsync(username);

            if (user is null)
            {
                return BadRequest("Usuário não encontrado.");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.MinValue;

            var result = await _user.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError(
                    "Falha ao revogar o refresh token do usuário {UserId}: {Errors}",
                    user.Id,
                    string.Join("; ", result.Errors.Select(error => error.Description)));

                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Não foi possível revogar a sessão.");
            }

            return NoContent();
        }


        //[HttpGet("ShowUsers")]
        //[Authorize(Policy = "Super")]
        //public async Task<ActionResult<ICollection<Usuario>>> ShowUsers()
        //{
        //    return Ok(await _user.Users.ToListAsync());

        //}


        //[HttpGet("ShowRoles")]
        //[Authorize(Policy = "Super")]
        //public async Task<ActionResult<ICollection<IdentityRole>>> ShowRoles()
        //{
        //    return Ok(await _role.Roles.ToListAsync());        
        //}

    }
}
