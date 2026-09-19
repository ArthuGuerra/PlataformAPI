using Application.DataTransferObject;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Infraestrutura.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTest.UnitTestes.Inscricoes
{
    public class InscricoesUnitTestServices : TestBase
    {

        private readonly Fixture _fix;

        public InscricoesUnitTestServices()
        {
            _fix = new Fixture();
        }


        [Fact]
        [Trait("Inscricao","Services")]
        public async Task GetIncricoesTeste()
        {
            // arrange
            var inscricao = _fix.Build<Inscricao>()
                .Without(x => x.Usuario)
                .Without(x => x.Evento)
                .CreateMany(10).ToList();

            var insRepo = new Mock<IUnitOfWork>();

            insRepo.Setup(r => r.InscricaoRepository.GetAllAsync()).ReturnsAsync(inscricao);

            var services = new InscricaoServices(_mapper, insRepo.Object);


            // Act

            var result = await services.GetAll();

            
            // Assert

            insRepo.Verify(r => r.InscricaoRepository.GetAllAsync(),Times.Once());
            Assert.Equal(inscricao.Count, result.Count);
                
        }


        [Fact]
        [Trait("Inscricao", "Services")]
        public async Task DeleteInscricaoTeste()
        {
            var inscricao = _fix.Build<Inscricao>()
                .Without(x => x.Usuario)
                .Without(x => x.Evento)
                .Create();

            var evento = _fix.Build<Evento>()
                .Without(x => x.Inscricoes)
                .Create();

            var insRepo = new Mock<IUnitOfWork>();

            insRepo.Setup(r => r.InscricaoRepository.GetIdAsync(inscricao.Id)).ReturnsAsync(inscricao);

            insRepo.Setup(r => r.EventosRepository.GetIdAsync(inscricao.EventoId))
                .ReturnsAsync(evento);

            insRepo.Setup(r => r.InscricaoRepository.Delete(It.IsAny<Inscricao>())).Returns(inscricao);

            insRepo.Setup(r => r.Commit()).Returns(Task.CompletedTask);


            var services = new InscricaoServices(_mapper, insRepo.Object);


            // act

            var result = await services.Delete(inscricao.Id);


            // Assert

            insRepo.Verify(r => r.InscricaoRepository.Delete(inscricao), Times.Once);
            Assert.True(result);
            



        }
    }
}
