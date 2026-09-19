using Application.Services;
using AutoFixture;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiTest.UnitTestes.Token
{
    public class TokenUnitTesteServices
    {       
        private readonly TokenService _tokenServices;

        public TokenUnitTesteServices()
        {
            _tokenServices = new TokenService();
        }



        private IConfiguration CreateConfiguration()
        {
            var settings = new Dictionary<string, string?>
            {
                ["JWT:SecretKey"] = "minha-chave-secreta-super-segura-com-pelo-menos-32-caracteres",
                ["JWT:TokenValidityInHours"] = "2",
                ["JWT:ValidAudience"] = "MinhaAPI",
                ["JWT:ValidIssuer"] = "MinhaAPI"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        

        private string CreateExpiredToken(IConfiguration config)
        {
            var key = config["JWT:SecretKey"]!;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Arthur"),
                new Claim(ClaimTypes.Email, "arthur@email.com"),
                new Claim(ClaimTypes.Role, "Administrador")
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: config["JWT:ValidIssuer"],
                audience: config["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(-1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateAccessToken_DeveGerarToken()
        {
            // Arrange
            var config = CreateConfiguration();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Arthur"),
                new Claim(ClaimTypes.Email, "arthur@email.com")
            };

            // Act
            var token = _tokenServices.GenerateAccessToken(claims, config);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token.RawData);
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateAccessToken_DeveConterClaims()
        {
            // Arrange
            var config = CreateConfiguration();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Arthur"),
                new Claim(ClaimTypes.Email, "arthur@email.com"),
                new Claim(ClaimTypes.Role, "Administrador")
            };

            // Act
            var token = _tokenServices.GenerateAccessToken(claims, config);

            // Assert
            Assert.Contains(
                token.Claims,
                x => x.Type == "unique_name" && x.Value == "Arthur");

            Assert.Contains(
                token.Claims,
                x => x.Type == "email" && x.Value == "arthur@email.com");

            Assert.Contains(
                token.Claims,
                x => x.Type == "role" && x.Value == "Administrador");
        }


        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateAccessToken_DeveConfigurarIssuerEAudience()
        {
            // Arrange
            var config = CreateConfiguration();

            var claims = new List<Claim>();

            // Act
            var token = _tokenServices.GenerateAccessToken(claims, config);

            // Assert
            Assert.Equal("MinhaAPI", token.Issuer);
            Assert.Equal("MinhaAPI", token.Audiences.Single());
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateAccessToken_DeveUsarHmacSha256()
        {
            // Arrange
            var config = CreateConfiguration();

            var claims = new List<Claim>();

            // Act
            var token = _tokenServices.GenerateAccessToken(claims, config);

            // Assert
            Assert.Equal(token.Header.Alg,"HS256");
        }


        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateAccessToken_DeveLancarExcecaoSemSecretKey()
        {
            // Arrange
            var settings = new Dictionary<string, string?>
            {
                ["JWT:TokenValidityInHours"] = "2",
                ["JWT:ValidAudience"] = "MinhaAPI",
                ["JWT:ValidIssuer"] = "MinhaAPI"
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();

            var claims = new List<Claim>();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                _tokenServices.GenerateAccessToken(claims, config));
        }


        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateRefreshToken_DeveGerarTokenValido()
        {
            // Act
            var token = _tokenServices.GenerateRefreshToken();

            // Assert
            Assert.NotNull(token);
            Assert.Equal(172, token.Length);
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateRefreshToken_DeveGerarTokensDiferentes()
        {
            // Act
            var token1 = _tokenServices.GenerateRefreshToken();
            var token2 = _tokenServices.GenerateRefreshToken();

            // Assert
            Assert.NotEqual(token1, token2);
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GetPrincipalFromExpiredToken_DeveRetornarPrincipal()
        {
            // Arrange
            var config = CreateConfiguration();

            var token = CreateExpiredToken(config);

            // Act
            var principal = _tokenServices.GetPrincipalFromExpiredToken(
                token,
                config);

            // Assert
            Assert.NotNull(principal);

            Assert.Equal(
                "Arthur",
                principal.FindFirst(ClaimTypes.Name)?.Value);

            Assert.Equal(
                "arthur@email.com",
                principal.FindFirst(ClaimTypes.Email)?.Value);

            Assert.Equal(
                "Administrador",
                principal.FindFirst(ClaimTypes.Role)?.Value);
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GetPrincipalFromExpiredToken_DeveFalharComChaveInvalida()
        {
            // Arrange
            var config = CreateConfiguration();

            var token = CreateExpiredToken(config);

            var invalidConfig = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["JWT:SecretKey"] =
                        "outra-chave-secreta-completamente-diferente-123456",
                    ["JWT:ValidAudience"] = "MinhaAPI",
                    ["JWT:ValidIssuer"] = "MinhaAPI"
                })
                .Build();

            // Act & Assert
            Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() =>
                _tokenServices.GetPrincipalFromExpiredToken(
                    token,
                    invalidConfig));
        }


        [Fact]
        [Trait("Auth", "Services")]
        public void GetPrincipalFromExpiredToken_DeveFalharComTokenInvalido()
        {
            // Arrange
            var config = CreateConfiguration();

            var token = "token completamente invalido";

            // Act & Assert
            Assert.Throws<SecurityTokenMalformedException>(() =>
                _tokenServices.GetPrincipalFromExpiredToken(
                    token,
                    config));
        }
    }
}
