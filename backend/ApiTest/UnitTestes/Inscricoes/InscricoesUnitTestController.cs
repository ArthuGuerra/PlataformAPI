using APISolution.Controllers;
using Application.DataTransferObject;
using Application.Interfaces;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace ApiTest.UnitTestes.Inscricoes
{
    public class InscricoesUnitTestController
    {
        private readonly Fixture _fix;       

        public InscricoesUnitTestController()
        {
            _fix = new Fixture();            
        }



        [Fact]
        [Trait("Inscricao", "Controller")]
        public async Task ListarInscricoesTeste()
        {
            // Arrange
            var userId = "usuario-123";

            var inscricoes = new List<InscricaoDTO>
            {
                _fix.Build<InscricaoDTO>()
                    .With(x => x.UsuarioId, userId)
                    .Create(),

                _fix.Build<InscricaoDTO>()
                    .With(x => x.UsuarioId, userId)
                    .Create(),

                _fix.Build<InscricaoDTO>()
                    .With(x => x.UsuarioId, "outro-usuario")
                    .Create()
            };

            var services = new Mock<IInscricaoServices>();

            services.Setup(x => x.GetAll()).ReturnsAsync(inscricoes);

            var claims = new List<Claim>
            {
                new Claim("userId", userId)
            };

            var identity = new ClaimsIdentity(
                claims,
                authenticationType: "TestAuthentication");

            var principal = new ClaimsPrincipal(identity);

            var controller = new InscricaoController(services.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = principal
                    }
                }
            };

            // Act
            var result = await controller.Get();

            // Assert
            Assert.NotNull(result.Value);

            var lista = result.Value.ToList();

            Assert.Equal(2, lista.Count);

            Assert.All(lista, inscricao =>
                Assert.Equal(userId, inscricao.UsuarioId));

            services.Verify(x => x.GetAll(), Times.Once);
        }



        [Fact]
        [Trait("Inscricao","Controller")]
        public async Task ListarInscricoesQuandoNaoexistirColecaoTeste()
        {
            // Arrange
            var userId = "usuario-123";

            var inscricoes = new List<InscricaoDTO>
            {
                _fix.Build<InscricaoDTO>()
                    .With(x => x.UsuarioId, "outro-usuario")
                    .Create()
            };

            var services = new Mock<IInscricaoServices>();

            services
                .Setup(x => x.GetAll())
                .ReturnsAsync(inscricoes);

            var claims = new List<Claim>
            {
                new Claim("userId", userId)
            };

            var principal = new ClaimsPrincipal(
                new ClaimsIdentity(claims, "TestAuthentication"));

            var controller = new InscricaoController(services.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = principal
                    }
                }
            };

            // Act
            var result = await controller.Get();

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);

            Assert.Equal(
                "Nenhuma Inscrição deste usuário",
                notFound.Value);
        }


        [Fact]
        [Trait("Inscricao", "Controller")]
        public async Task DeleteInscricaoTeste()
        {
            // Arrange
            int id = 10;

            var services = new Mock<IInscricaoServices>();

            services.Setup(x => x.Delete(id)).ReturnsAsync(true);

            var controller = new InscricaoController(services.Object);

            // Act
            var result = await controller.DeleteIns(id);

            // Assert
            services.Verify(x => x.Delete(id),Times.Once);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal($"Inscrição do evento: {id} cancelada!",
                okResult.Value);
        }


    }
}
