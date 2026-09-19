using APISolution.Controllers;
using Application.DataTransferObject;
using Application.Interfaces;
using AutoFixture;
using Domain.Entities;
using Infraestrutura.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTest.UnitTestes.Eventos
{
    public class EventosUnitTestController : TestBase
    {

        private readonly Fixture _fix;

        public EventosUnitTestController()
        {
            _fix = new Fixture();
        }



        [Fact]
        [Trait("Evento","Controller")]
        public async Task ListarEventosControllerTeste()
        {

            //Arrange
            var eventos = _fix.Build<EventoDTO>()
               .CreateMany(5).ToList();

            var eventoRepo = new Mock<IEventoServices>();

            eventoRepo.Setup(r => r.ListarEventos()).ReturnsAsync(eventos);

            var controller = new EventoController(eventoRepo.Object);

            // Act
            var result = await controller.GetAll();

            // Assert

            // Assert
            eventoRepo.Verify(r => r.ListarEventos(), Times.Once);
            Assert.NotNull(result);
           
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            
            Assert.Equal(200, okResult.StatusCode);
        
            var model = Assert.IsAssignableFrom<IEnumerable<EventoDTO>>(okResult.Value);
         
            Assert.Equal(eventos, okResult.Value);


        }

        [Fact]
        [Trait("Evento", "Controller")]
        public async Task GetIdEventosControllerTeste()
        {
            //Arrange
            var evento = _fix.Build<EventoDTO>()
                .Create();

            var services = new Mock<IEventoServices>();

            services.Setup(r => r.GetEventoId(evento.Id)).ReturnsAsync(evento);


            // Act
            var controller = new EventoController(services.Object);

            var result = await controller.GetId(evento.Id);


            // Assert
            services.Verify(r => r.GetEventoId(evento.Id), Times.Once);
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal(evento, okResult.Value);


        }


        [Fact]
        [Trait("Evento", "Controller")]
        public async Task GetNomeEventosControllerTeste()
        {
            //Arrange
            var evento = _fix.Build<EventoDTO>()
                .Create();

            var services = new Mock<IEventoServices>();

            services.Setup(r => r.GetEventoNomes(evento.Nome)).ReturnsAsync(evento);


            var controller = new EventoController(services.Object);

            // Act
            var result = await controller.GetNome(evento.Nome);


            // Assert
            services.Verify(r => r.GetEventoNomes(evento.Nome), Times.Once);
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal(evento, okResult.Value);




        }



        [Fact]
        [Trait("Evento", "Controller")]
        public async Task CreateEventosControllerTeste()
        {
            //Arrange
            var evento = _fix.Build<EventoDTO>()
                .Create();

            var services = new Mock<IEventoServices>();

            services.Setup(r => r.CreateEvento(evento)).ReturnsAsync(true);


            var controller = new EventoController(services.Object);

            // Act
            var result = await controller.CreateEventos(evento);


            // Assert
            services.Verify(r => r.CreateEvento(evento), Times.Once);
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.True((bool)okResult.Value!);

        }



        [Fact]
        [Trait("Evento", "Controller")]
        public async Task UpdateADMEventosControllerTeste()
        {
            //Arrange
            var evento = _fix.Build<EventoDTO>()
                .Create();

            var create = _fix.Build<CreateEventoDTO>()
                .Create();


            var services = new Mock<IEventoServices>();

            services.Setup(r => r.AtualizarEventoADM(evento.Id,create)).ReturnsAsync(true);


            var controller = new EventoController(services.Object);

            // Act
            var result = await controller.UpdateADM(evento.Id, create);


            // Assert
            services.Verify(r => r.AtualizarEventoADM(evento.Id, create), Times.Once);
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.True((bool)okResult.Value!);

        }



        [Fact]
        [Trait("Evento", "Controller")]
        public async Task UpdateEventosControllerTeste()
        {
            //Arrange
            var evento = _fix.Build<EventoDTO>()
                .Create();            


            var services = new Mock<IEventoServices>();

            services.Setup(r => r.AtualizarEvento(evento.Id, evento)).ReturnsAsync(true);


            var controller = new EventoController(services.Object);

            // Act
            var result = await controller.UpdateEvento(evento.Id, evento);


            // Assert
            services.Verify(r => r.AtualizarEvento(evento.Id, evento), Times.Once);
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.True((bool)okResult.Value!);

        }



        [Fact]
        [Trait("Evento", "Controller")]
        public async Task DeleteEventosControllerTeste()
        {
            //Arrange
            var evento = _fix.Build<EventoDTO>()
                .Create();


            var services = new Mock<IEventoServices>();

            services.Setup(r => r.DeletarEventos(evento.Id)).ReturnsAsync(true);


            var controller = new EventoController(services.Object);

            // Act
            var result = await controller.DeleteEvento(evento.Id);


            // Assert
            services.Verify(r => r.DeletarEventos(evento.Id), Times.Once);
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.True((bool)okResult.Value!);

        }



        [Fact]
        [Trait("Evento", "Controller")]
        public async Task FazerInscricaoEventosControllerTeste()
        {
            //Arrange
            var evento = _fix.Build<EventoDTO>()
                .Create();

            var ins = _fix.Build<InscricaoDTO>().Create();


            var services = new Mock<IEventoServices>();

            services.Setup(r => r.Inscricao(evento.Id, ins)).ReturnsAsync(true);


            var controller = new EventoController(services.Object);

            // Act
            var result = await controller.FazerInscricao(evento.Id, ins);


            // Assert
            services.Verify(r => r.Inscricao(evento.Id, ins), Times.Once);
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.True((bool)okResult.Value!);

        }




        [Fact]
        [Trait("Evento", "Controller")]
        public async Task ListarEventosDefatoControllerTeste()
        {

            //Arrange
            var eventos = _fix.Build<Evento>()
                .Without(x => x.Inscricoes)                
               .CreateMany(5).ToList();

            var eventoRepo = new Mock<IEventoServices>();

            eventoRepo.Setup(r => r.EventoInscricao()).ReturnsAsync(eventos);

            var controller = new EventoController(eventoRepo.Object);

            // Act
            var result = await controller.EventoInscricoes();


            // Assert
            eventoRepo.Verify(r => r.EventoInscricao(), Times.Once);
            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);

            var model = Assert.IsAssignableFrom<IEnumerable<Evento>>(okResult.Value);

            Assert.Equal(eventos, okResult.Value);


        }



    }
}
