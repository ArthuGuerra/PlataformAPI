using Application.DataTransferObject;
using Application.Interfaces;
using Application.Services;
using AutoFixture;
using Bogus;
using Domain.Entities;
using Infraestrutura.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTest.UnitTestes.Eventos
{
    
    public class EventosUnitTestServices : TestBase
    {
        private const string _idioma = "pt_BR";
        private readonly Fixture _fix;

        public EventosUnitTestServices()
        {
            _fix = new Fixture();
        }


        [Fact]
        [Trait("Evento", "Services")]
        public async Task ListarEventosTeste()
        {
            // Arrange
            var eventos = _fix.Build<EventoDTO>()
                .CreateMany(5).ToList();

            var eventoRepo = new Mock<IUnitOfWork>();

            var map = _mapper.Map<ICollection<Evento>>(eventos);


            eventoRepo.Setup(r => r.EventosRepository.GetAllAsync())
               .ReturnsAsync(map);


            var services = new EventosServices(eventoRepo.Object, _mapper);
            

            // Act
            var result = await services.ListarEventos();


            // Assert            
            eventoRepo.Verify(r => r.EventosRepository.GetAllAsync(),Times.Once);
            Assert.NotNull(result);
            Assert.Equal(eventos.Count, result.Count);
        }



        [Fact]
        [Trait("Evento", "Services")]
        public async Task GetEventoIdTeste()
        {
            // arrange
            EventoDTO evento = new Faker<EventoDTO>(_idioma)               
                .RuleFor(x => x.Descricao, f => f.Lorem.Sentence())
                .RuleFor(x => x.LocalEvento, f => f.Lorem.Sentence())
                .RuleFor(x => x.Imagem, f => f.Lorem.Sentence())
                .RuleFor(x => x.Nome, f => f.Name.FirstName())
                .RuleFor(x => x.Id, f => f.IndexFaker)
                .Generate();


            var eventoRepo = new Mock<IUnitOfWork>();

            var map = _mapper.Map<Evento>(evento);

            eventoRepo.Setup(r => r.EventosRepository.GetIdAsync(map.Id))
                .ReturnsAsync(map);

            var services = new EventosServices(eventoRepo.Object, _mapper);


            // Act
            var result = await services.GetEventoId(map.Id);


            //Assert
            eventoRepo.Verify(r => r.EventosRepository.GetIdAsync(map.Id), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(map.Id, result.Id);
            
        }


        [Fact]
        [Trait("Evento", "Services")]
        public async Task getEventoNomeTeste()
        {
            // arrange

            EventoDTO evento = new Faker<EventoDTO>(_idioma)
              .RuleFor(x => x.Descricao, f => f.Lorem.Sentence())
              .RuleFor(x => x.LocalEvento, f => f.Lorem.Sentence())
              .RuleFor(x => x.Imagem, f => f.Lorem.Sentence())
              .RuleFor(x => x.Nome, f => f.Name.FirstName())
              .RuleFor(x => x.Id, f => f.IndexFaker)
              .Generate();


            var eventoRepo = new Mock<IUnitOfWork>();           

            var map = _mapper.Map<Evento>(evento);


            eventoRepo.Setup(r => r.EventosRepository.GetEventoNome(map.Nome)).ReturnsAsync(map);


            var services = new EventosServices(eventoRepo.Object, _mapper);


            // act

            var result = await services.GetEventoNomes(map.Nome);


            // Assert

            eventoRepo.Verify(r => r.EventosRepository.GetEventoNome(map.Nome),Times.Once);
            Assert.Equal(result.Nome, evento.Nome);
            Assert.Equal(result.Id, evento.Id);
        }





        [Fact]
        [Trait("Evento", "Services")]
        public async Task Cadastrar_Evento_Corretamente()
        {
            // arrange
            var evento = new Faker<EventoDTO>(_idioma)                
                .RuleFor(x => x.Descricao, f => f.Lorem.Sentence())
                .RuleFor(x => x.LocalEvento, f => f.Lorem.Sentence())
                .RuleFor(x => x.Imagem, f => f.Lorem.Sentence())
                .RuleFor(x => x.Nome, f => f.Name.FirstName())
                .RuleFor(x => x.Id, f => f.IndexFaker)
                .Generate();            


            var eventoRepo = new Mock<IUnitOfWork>();

            var map = _mapper.Map<Evento>(evento);

            eventoRepo.Setup(r => r.EventosRepository.GetAllAsync())
                .ReturnsAsync(new List<Evento>());

            
            eventoRepo.Setup(r => r.EventosRepository.Create(It.IsAny<Evento>()))
                .Returns(map);

            eventoRepo.Setup(r => r.Commit()).Returns(Task.CompletedTask);


            var services = new EventosServices(eventoRepo.Object, _mapper);


            // act 
            var result = await services.CreateEvento(evento);


            // assert
            eventoRepo.Verify(r => r.EventosRepository
            .Create(It.IsAny<Evento>()),Times.Once);

            Assert.NotNull(evento);
            Assert.True(result);            

        }



        [Fact]
        [Trait("Evento", "Services")]
        public async Task AtualizarEventoTesteADM()
        {         

            var evento = _fix.Build<Evento>()
                .Without(x => x.Inscricoes)
                .Create();

            var dto = _fix.Build<CreateEventoDTO>().Create();
                

            var eventoRepo = new Mock<IUnitOfWork>();

            eventoRepo.Setup(r => r.EventosRepository.UpdateADM(evento.Id, dto.Preco, dto.QuantidadeDeKitsDisponiveis)).ReturnsAsync(true);


            var services = new EventosServices(eventoRepo.Object, _mapper);


            // Act

            var result = await services.AtualizarEventoADM(evento.Id, dto);


            // Assert

            eventoRepo.Verify(r => r.EventosRepository.UpdateADM(evento.Id, dto.Preco, dto.QuantidadeDeKitsDisponiveis), Times.Once);
            Assert.True(result);

        }


        [Fact]
        [Trait("Evento", "Services")]
        public async Task UpdateEventoNormalTeste()
        {
            // arrange

            var evento = new Faker<EventoDTO>(_idioma)
               .RuleFor(x => x.Descricao, f => f.Lorem.Sentence())
               .RuleFor(x => x.LocalEvento, f => f.Lorem.Sentence())
               .RuleFor(x => x.Imagem, f => f.Lorem.Sentence())
               .RuleFor(x => x.Nome, f => f.Name.FirstName())
               .RuleFor(x => x.Id, f => f.IndexFaker)
               .Generate();

            var map = _mapper.Map<Evento>(evento);

            var eventoRepo = new Mock<IUnitOfWork>();

            eventoRepo.Setup(r => r.EventosRepository.GetIdAsync(evento.Id)).ReturnsAsync(map);

            eventoRepo.Setup(r => r.EventosRepository.Update(It.IsAny<Evento>())).Returns(map);

            eventoRepo.Setup(r => r.Commit()).Returns(Task.CompletedTask);


            // Act

            var services = new EventosServices(eventoRepo.Object, _mapper);

            var result = await services.AtualizarEvento(evento.Id, evento);

            // Assert

            eventoRepo.Verify(r => r.EventosRepository.Update(It.IsAny<Evento>()),Times.Once);
            Assert.True(result);

        }


        [Fact]
        [Trait("Evento", "Services")]
        public async Task DeleteEventoTeste()
        {
            //Arrange
            var evento = new Faker<EventoDTO>(_idioma)
             .RuleFor(x => x.Descricao, f => f.Lorem.Sentence())
             .RuleFor(x => x.LocalEvento, f => f.Lorem.Sentence())
             .RuleFor(x => x.Imagem, f => f.Lorem.Sentence())
             .RuleFor(x => x.Nome, f => f.Name.FirstName())
             .RuleFor(x => x.Id, f => f.IndexFaker)
             .Generate();

            var eventoRepo = new Mock<IUnitOfWork>();

            var map = _mapper.Map<Evento>(evento);


            eventoRepo.Setup(r => r.EventosRepository.GetIdAsync(evento.Id)).ReturnsAsync(map);

            eventoRepo.Setup(r => r.EventosRepository.Delete(It.IsAny<Evento>())).Returns(map);

            eventoRepo.Setup(r => r.Commit()).Returns(Task.CompletedTask);

            var services = new EventosServices(eventoRepo.Object, _mapper);


            // Act

            var result = await services.DeletarEventos(evento.Id);

            // Assert

            eventoRepo.Verify(r => r.EventosRepository.Delete(map), Times.Once);
            Assert.True(result);

        }


        [Fact]
        [Trait("Evento", "Services")]
        public async Task FazerInscricaoTeste()
        {
            // arrange
            var evento = new Faker<EventoDTO>(_idioma)
                .RuleFor(x => x.Descricao, f => f.Lorem.Sentence())
                .RuleFor(x => x.LocalEvento, f => f.Lorem.Sentence())
                .RuleFor(x => x.Imagem, f => f.Lorem.Sentence())
                .RuleFor(x => x.Nome, f => f.Name.FirstName())
                .RuleFor(x => x.Id, f => f.IndexFaker)
                .Generate();

            var inscricao = _fix.Build<Inscricao>()
                .Without(x => x.Usuario)
                .Without(x => x.Evento)
                .Create();

            var map = _mapper.Map<InscricaoDTO>(inscricao);

            var eventoRepo = new Mock<IUnitOfWork>();

            eventoRepo.Setup(r => r.EventosRepository.FazerInscricao(evento.Id, It.IsAny<Inscricao>())).ReturnsAsync(true);


            // Act
            var services = new EventosServices(eventoRepo.Object, _mapper);

            var result = await services.Inscricao(evento.Id, map);

            //Assert

            eventoRepo.Verify(r => r.EventosRepository.FazerInscricao(evento.Id, It.IsAny<Inscricao>()), Times.Once());
            Assert.True(result);


        }


        [Fact]
        [Trait("Evento", "Services")]
        public async Task ListarEventoTESTE()
        {
            // Arrange
            var eventos = _fix.Build<Evento>()
                .Without(x => x.Inscricoes)
                .CreateMany(5).ToList();

            var eventoRepo = new Mock<IUnitOfWork>();            


            eventoRepo.Setup(r => r.EventosRepository.GetEventoInscricao())
               .ReturnsAsync(eventos);


            var services = new EventosServices(eventoRepo.Object, _mapper);


            // Act
            var result = await services.EventoInscricao();


            // Assert            
            eventoRepo.Verify(r => r.EventosRepository.GetEventoInscricao(), Times.Once);
            Assert.NotNull(result);
            Assert.Equal(eventos.Count, result.Count);
        }



    }
}

