using APISolution.Controllers;
using Application.DataTransferObject;
using Application.DataTransferObject.IdentityDTO;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiTest.UnitTestes.Token
{
    public class TokenUnitTestController
    {


        private readonly Mock<ITokenService> _token;
        private readonly Mock<UserManager<Usuario>> _user;
        private readonly Mock<RoleManager<IdentityRole>> _role;
        private readonly Mock<IConfiguration> _config;
        private readonly Mock<ILogger<AuthController>> _logger;
        private readonly Mock<IUsuarioServices> _userServices;

        public TokenUnitTestController()
        {
            _token = new Mock<ITokenService>();

            _user = CriarMockUserManager();

            _role = CriarMockRoleManager();

            _config = new Mock<IConfiguration>();
            _logger = new Mock<ILogger<AuthController>>();
            _userServices = new Mock<IUsuarioServices>();
        }

        private AuthController CriarController()
        {
            return new AuthController(
                _token.Object,
                _user.Object,
                _role.Object,
                _config.Object,
                _logger.Object,
                _userServices.Object);
        }

        private static Mock<UserManager<Usuario>> CriarMockUserManager()
        {
            var userStore = new Mock<IUserStore<Usuario>>();

            return new Mock<UserManager<Usuario>>(
                userStore.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);
        }

        private static Mock<RoleManager<IdentityRole>> CriarMockRoleManager()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            return new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                null!,
                null!,
                null!,
                null!);
        }

        private IConfiguration CriarConfiguracao()
        {
            var settings = new Dictionary<string, string?>
            {
                ["JWT:RefreshTokenValidityInHours"] = "2"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }



        [Fact]
        [Trait("Auth", "Controller")] 
        public async Task Login_DeveRetornarOk_QuandoUsuarioEsenhaForemValidos()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = "usuario-123",
                UserName = "arthur",
                Email = "arthur@email.com"
            };

            var model = new LoginModelDTO
            {
                Username = "arthur",
                Password = "123456"
            };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.UserName!),
                new Claim(ClaimTypes.Email, usuario.Email!),
                new Claim("userId", usuario.Id!)
            };

            var jwtToken = new JwtSecurityToken(
                issuer: "MinhaAPI",
                audience: "MinhaAPI",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2));

            var refreshToken = "refresh-token-teste";

            _config
                .Setup(x => x["JWT:RefreshTokenValidityInHours"])
                .Returns("2");

            _user
                .Setup(x => x.FindByNameAsync(model.Username))
                .ReturnsAsync(usuario);

            _user
                .Setup(x => x.CheckPasswordAsync(usuario, model.Password))
                .ReturnsAsync(true);

            _user
                .Setup(x => x.GetRolesAsync(usuario))
                .ReturnsAsync(new List<string> { "User" });

            _user
                .Setup(x => x.UpdateAsync(usuario))
                .ReturnsAsync(IdentityResult.Success);

            _token
                .Setup(x => x.GenerateAccessToken(
                    It.IsAny<List<Claim>>(),
                    It.IsAny<IConfiguration>()))
                .Returns(jwtToken);

            _token
                .Setup(x => x.GenerateRefreshToken())
                .Returns(refreshToken);

            var controller = CriarController();

            // Act
            var result = await controller.Login(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            Assert.Equal(refreshToken, usuario.RefreshToken);

            _user.Verify(
                x => x.FindByNameAsync(model.Username),
                Times.Once);

            _user.Verify(
                x => x.CheckPasswordAsync(usuario, model.Password),
                Times.Once);

            _user.Verify(
                x => x.GetRolesAsync(usuario),
                Times.Once);

            _user.Verify(
                x => x.UpdateAsync(usuario),
                Times.Once);

            _token.Verify(
                x => x.GenerateAccessToken(
                    It.IsAny<List<Claim>>(),
                    It.IsAny<IConfiguration>()),
                Times.Once);

            _token.Verify(
                x => x.GenerateRefreshToken(),
                Times.Once);
        }

        [Fact]
        [Trait("Auth", "Controller")]
        public async Task Login_DeveRetornarUnauthorized_QuandoUsuarioNaoExistir()
        {
            // Arrange
            var model = new LoginModelDTO
            {
                Username = "usuario-inexistente",
                Password = "123456"
            };

            _user
                .Setup(x => x.FindByNameAsync(model.Username))
                .ReturnsAsync((Usuario?)null);

            var controller = CriarController();

            // Act
            var result = await controller.Login(model);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);

            _user.Verify(
                x => x.FindByNameAsync(model.Username),
                Times.Once);

            _user.Verify(
                x => x.CheckPasswordAsync(
                    It.IsAny<Usuario>(),
                    It.IsAny<string>()),
                Times.Never);
        }



        [Fact]
        [Trait("Auth", "Controller")]
        public async Task Login_DeveRetornarUnauthorized_QuandoSenhaForInvalida()
        {
            // Arrange
            var model = new LoginModelDTO
            {
                Username = "arthur",
                Password = "senha-incorreta"
            };

            var usuario = new Usuario
            {
                Id = "usuario-123",
                UserName = "arthur",
                Email = "arthur@email.com"
            };

            _user
                .Setup(x => x.FindByNameAsync(model.Username))
                .ReturnsAsync(usuario);

            _user
                .Setup(x => x.CheckPasswordAsync(usuario, model.Password))
                .ReturnsAsync(false);

            var controller = CriarController();

            // Act
            var result = await controller.Login(model);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);

            _user.Verify(
                x => x.CheckPasswordAsync(usuario, model.Password),
                Times.Once);

            _token.Verify(
                x => x.GenerateAccessToken(
                    It.IsAny<List<Claim>>(),
                    It.IsAny<IConfiguration>()),
                Times.Never);
        }

        [Fact]
        [Trait("Auth", "Controller")]
        public async Task Register_DeveRetornarOk_QuandoUsuarioForCriado()
        {
            // Arrange
            var model = new RegisterModelDTO
            {
                Username = "arthur",
                Email = "arthur@email.com",
                Password = "123456",
                CPF = "12345678900",
                PhoneNumber = "11999999999"
            };

            _userServices.Setup(x => x.GetAllUsers()).ReturnsAsync(new List<UsuarioPrintDTO>());

            _userServices
                .Setup(x => x.NormalizeNome(It.IsAny<string>()))
                .Returns<string>(valor => valor.ToLower());

            _user
                .Setup(x => x.CreateAsync(
                    It.IsAny<Usuario>(),
                    model.Password))
                .ReturnsAsync(IdentityResult.Success);

            _user
                .Setup(x => x.AddToRoleAsync(
                    It.IsAny<Usuario>(),
                    "User"))
                .ReturnsAsync(IdentityResult.Success);

            var controller = CriarController();

            // Act
            var result = await controller.Register(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);

            _user.Verify(
                x => x.CreateAsync(
                    It.Is<Usuario>(usuario =>
                        usuario.UserName == model.Username &&
                        usuario.Email == model.Email &&
                        usuario.CPF == model.CPF),
                    model.Password),
                Times.Once);

            _user.Verify(
                x => x.AddToRoleAsync(
                    It.IsAny<Usuario>(),
                    "User"),
                Times.Once);
        }

        [Fact]
        [Trait("Auth", "Controller")]
        public async Task Register_DeveRetornarBadRequest_QuandoUsuarioJaExistir()
        {
            // Arrange
            var model = new RegisterModelDTO
            {
                Username = "arthur",
                Email = "arthur@email.com",
                Password = "123456",
                CPF = "12345678900"
            };

            var usuarioExistente = new UsuarioPrintDTO
            {
                UserName = "arthur",
                Email = "arthur@email.com",
                CPF = "12345678900"
            };

            _userServices
                .Setup(x => x.GetAllUsers())
                .ReturnsAsync(new List<UsuarioPrintDTO>
                {
                    usuarioExistente
                });

            _userServices
                .Setup(x => x.NormalizeNome(It.IsAny<string>()))
                .Returns<string>(valor => valor.ToLower());

            var controller = CriarController();

            // Act
            var result = await controller.Register(model);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal(
                "Usuario, Email ou CPF existente!",
                badRequest.Value);

            _user.Verify(
                x => x.CreateAsync(
                    It.IsAny<Usuario>(),
                    It.IsAny<string>()),
                Times.Never);
        }



        [Fact]
        [Trait("Auth", "Controller")]
        public async Task Register_DeveRetornarInternalServerError_QuandoCreateFalhar()
        {
            // Arrange
            var model = new RegisterModelDTO
            {
                Username = "arthur",
                Email = "arthur@email.com",
                Password = "123456",
                CPF = "12345678900"
            };

            _userServices
                .Setup(x => x.GetAllUsers())
                .ReturnsAsync(new List<UsuarioPrintDTO>());

            _userServices
                .Setup(x => x.NormalizeNome(It.IsAny<string>()))
                .Returns<string>(valor => valor.ToLower());

            _user
                .Setup(x => x.CreateAsync(
                    It.IsAny<Usuario>(),
                    model.Password))
                .ReturnsAsync(
                    IdentityResult.Failed(
                        new IdentityError
                        {
                            Code = "PasswordTooShort",
                            Description = "Senha muito curta"
                        }));

            var controller = CriarController();

            // Act
            var result = await controller.Register(model);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);

            Assert.Equal(
                StatusCodes.Status500InternalServerError,
                statusCodeResult.StatusCode);

            _user.Verify(
                x => x.CreateAsync(
                    It.IsAny<Usuario>(),
                    model.Password),
                Times.Once);

            _user.Verify(
                x => x.AddToRoleAsync(
                    It.IsAny<Usuario>(),
                    "User"),
                Times.Never);
        }



        [Fact]
        [Trait("Auth", "Controller")]
        public async Task CreateRole_DeveRetornarCreated_QuandoRoleNaoExistir()
        {
            // Arrange
            var roleName = "Admin";

            _role
                .Setup(x => x.RoleExistsAsync(roleName))
                .ReturnsAsync(false);

            _role
                .Setup(x => x.CreateAsync(
                    It.Is<IdentityRole>(role => role.Name == roleName)))
                .ReturnsAsync(IdentityResult.Success);

            var controller = CriarController();

            // Act
            var result = await controller.CreateRole(roleName);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(201, objectResult.StatusCode);

            Assert.Equal(
                $"Status: Success; Message: Role {roleName} added successfully",
                objectResult.Value);

            _role.Verify(
                x => x.RoleExistsAsync(roleName),
                Times.Once);

            _role.Verify(
                x => x.CreateAsync(
                    It.Is<IdentityRole>(role => role.Name == roleName)),
                Times.Once);
        }


        [Fact]
        [Trait("Auth", "Controller")]
        public async Task CreateRole_DeveRetornarBadRequest_QuandoRoleJaExistir()
        {
            // Arrange
            var roleName = "Admin";

            _role
                .Setup(x => x.RoleExistsAsync(roleName))
                .ReturnsAsync(true);

            var controller = CriarController();

            // Act
            var result = await controller.CreateRole(roleName);

            // Assert
            var badRequest = Assert.IsType<ObjectResult>(result);

            Assert.Equal(400, badRequest.StatusCode);

            Assert.Equal(
                $"Status: Error; Message: Role {roleName} já existe",
                badRequest.Value);

            _role.Verify(
                x => x.CreateAsync(It.IsAny<IdentityRole>()),
                Times.Never);
        }

        [Fact]
        [Trait("Auth", "Controller")]
        public async Task AddUserRole_DeveRetornarCreated_QuandoUsuarioForAdicionado()
        {
            // Arrange
            var email = "arthur@email.com";
            var roleName = "User";

            var usuario = new Usuario
            {
                Email = email,
                UserName = "arthur"
            };

            _user
                .Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(usuario);

            _user
                .Setup(x => x.IsInRoleAsync(usuario, roleName))
                .ReturnsAsync(false);

            _user
                .Setup(x => x.AddToRoleAsync(usuario, roleName))
                .ReturnsAsync(IdentityResult.Success);

            var controller = CriarController();

            // Act
            var result = await controller.AddUserRole(email, roleName);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(201, objectResult.StatusCode);

            _user.Verify(
                x => x.FindByEmailAsync(email),
                Times.Once);

            _user.Verify(
                x => x.IsInRoleAsync(usuario, roleName),
                Times.Once);

            _user.Verify(
                x => x.AddToRoleAsync(usuario, roleName),
                Times.Once);
        }

        [Fact]
        [Trait("Auth", "Controller")]
        public async Task AddUserRole_DeveRetornarBadRequest_QuandoUsuarioJaPossuirRole()
        {
            // Arrange
            var email = "arthur@email.com";
            var roleName = "User";

            var usuario = new Usuario
            {
                Email = email,
                UserName = "arthur"
            };

            _user
                .Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(usuario);

            _user
                .Setup(x => x.IsInRoleAsync(usuario, roleName))
                .ReturnsAsync(true);

            var controller = CriarController();

            // Act
            var result = await controller.AddUserRole(email, roleName);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal(
                $"Usuario: {usuario.Email} já possui a role {roleName}",
                badRequest.Value);

            _user.Verify(
                x => x.AddToRoleAsync(
                    It.IsAny<Usuario>(),
                    It.IsAny<string>()),
                Times.Never);
        }


        [Fact]
        [Trait("Auth", "Controller")]
        public async Task RefreshToken_DeveRetornarBadRequest_QuandoTokenForInvalido()
        {
            // Arrange
            var model = new TokenModelDTO
            {
                AccessToken = "access-token-invalido",
                RefreshToken = "refresh-token-invalido"
            };

            var config = CriarConfiguracao();

            _token
                .Setup(x => x.GetPrincipalFromExpiredToken(
                    model.AccessToken,
                    config))
                .Returns((ClaimsPrincipal?)null);

            var controller = CriarController();

            // Act
            var result = await controller.RefreshToken(model);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal(
                "Access/Refresh Token inválido",
                badRequest.Value);
        }
    }
}
