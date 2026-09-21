using Application.Configuration;
using Application.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace ApiTest.UnitTestes.Token
{
    public class TokenUnitTesteServices
    {
        private const string SecretKey =
            "minha-chave-secreta-super-segura-com-pelo-menos-32-caracteres";

        private const string Issuer = "MinhaAPI";
        private const string Audience = "MinhaAPI";

        private readonly TokenService _tokenServices;

        public TokenUnitTesteServices()
        {
            _tokenServices = CriarTokenService();
        }

        private static JwtOptions CriarJwtOptions()
        {
            return new JwtOptions
            {
                SecretKey = SecretKey,
                TokenValidityInHours = 2,
                RefreshTokenValidityInHours = 2,
                ValidIssuer = Issuer,
                ValidAudience = Audience
            };
        }

        private static TokenService CriarTokenService()
        {
            var options = Options.Create(CriarJwtOptions());

            return new TokenService(options);
        }

        private static TokenService CriarTokenService(JwtOptions jwtOptions)
        {
            var options = Options.Create(jwtOptions);

            return new TokenService(options);
        }

        private static string CreateExpiredToken()
        {
            var jwtOptions = CriarJwtOptions();

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "Arthur"),
                new(ClaimTypes.Email, "arthur@email.com"),
                new(ClaimTypes.Role, "Administrador")
            };

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtOptions.ValidIssuer,
                audience: jwtOptions.ValidAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(-1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateAccessToken_DeveGerarToken()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "Arthur"),
                new(ClaimTypes.Email, "arthur@email.com")
            };

            // Act
            var token = _tokenServices.GenerateAccessToken(claims);

            // Assert
            Assert.NotNull(token);
            Assert.False(string.IsNullOrWhiteSpace(token.RawData));
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateAccessToken_DeveConterClaims()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, "Arthur"),
                new(ClaimTypes.Email, "arthur@email.com"),
                new(ClaimTypes.Role, "Administrador")
            };

            // Act
            var token = _tokenServices.GenerateAccessToken(claims);

            // Assert
            Assert.Contains(
                token.Claims,
                claim =>
                    claim.Type == ClaimTypes.Name &&
                    claim.Value == "Arthur");

            Assert.Contains(
                token.Claims,
                claim =>
                    claim.Type == ClaimTypes.Email &&
                    claim.Value == "arthur@email.com");

            Assert.Contains(
                token.Claims,
                claim =>
                    claim.Type == ClaimTypes.Role &&
                    claim.Value == "Administrador");
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateAccessToken_DeveConfigurarIssuerEAudience()
        {
            // Arrange
            var claims = new List<Claim>();

            // Act
            var token = _tokenServices.GenerateAccessToken(claims);

            // Assert
            Assert.Equal(Issuer, token.Issuer);
            Assert.Equal(Audience, token.Audiences.Single());
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateAccessToken_DeveUsarHmacSha256()
        {
            // Arrange
            var claims = new List<Claim>();

            // Act
            var token = _tokenServices.GenerateAccessToken(claims);

            // Assert
            Assert.Equal(
                SecurityAlgorithms.HmacSha256,
                token.Header.Alg);
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GenerateRefreshToken_DeveGerarTokenValido()
        {
            // Act
            var token = _tokenServices.GenerateRefreshToken();

            // Assert
            Assert.NotNull(token);

            // 128 bytes convertidos em Base64 resultam normalmente
            // em 172 caracteres.
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
            var token = CreateExpiredToken();

            // Act
            var principal =
                _tokenServices.GetPrincipalFromExpiredToken(token);

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
        public void GetPrincipalFromExpiredToken_DeveAceitarTokenExpirado()
        {
            // Arrange
            var token = CreateExpiredToken();

            // Act
            var principal =
                _tokenServices.GetPrincipalFromExpiredToken(token);

            // Assert
            Assert.NotNull(principal);
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GetPrincipalFromExpiredToken_DeveFalharComChaveInvalida()
        {
            // Arrange
            var token = CreateExpiredToken();

            var tokenServiceComChaveDiferente =
                CriarTokenService(new JwtOptions
                {
                    SecretKey =
                        "outra-chave-secreta-completamente-diferente-123456",

                    TokenValidityInHours = 2,
                    RefreshTokenValidityInHours = 2,
                    ValidIssuer = Issuer,
                    ValidAudience = Audience
                });

            // Act & Assert
            Assert.Throws<SecurityTokenException>(() =>
                tokenServiceComChaveDiferente
                    .GetPrincipalFromExpiredToken(token));
        }

        [Fact]
        [Trait("Auth", "Services")]
        public void GetPrincipalFromExpiredToken_DeveFalharComTokenInvalido()
        {
            // Arrange
            var token = "token completamente invalido";

            // Act & Assert
            Assert.Throws<SecurityTokenException>(() =>
                _tokenServices.GetPrincipalFromExpiredToken(token));
        }
    }
}