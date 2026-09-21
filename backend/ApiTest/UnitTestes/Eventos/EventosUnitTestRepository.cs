using Application.Interfaces;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Infraestrutura.BancoContexto;
using Infraestrutura.ContextRepository;
using Infraestrutura.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ApiTest.UnitTestes.Eventos
{
    public class EventosUnitTestRepository : TestBase
    {

        private readonly Fixture _fix;

        public EventosUnitTestRepository()
        {
            _fix = new Fixture();
        }


        [Fact]
        [Trait("Evento","Repository")]
        public async Task ListarEventosRepositoryTeste()
        {
            // Arrange

            var context = CreateContext(DatabaseType.InMemory);

            var evento = _fix.Build<Evento>()
                .Without(x => x.Inscricoes)
                .Create();

            var inscricao = _fix.Build<Inscricao>()
                .Without(x => x.Evento)
                .Without(x => x.Usuario)
                .Create();

            evento.Inscricoes = new List<Inscricao>
            {
                inscricao
            };


            await context.Evento.AddAsync(evento);
            await context.SaveChangesAsync();


            context.ChangeTracker.Clear();

            var repo = new EventosRepositorio(context);

            //Act

            var result = await repo.GetEventoInscricao();


            // Assert

            Assert.NotNull(result);
            Assert.Single(result);

            var eventoRetornado = result.First();

            Assert.NotNull(eventoRetornado.Inscricoes);
            Assert.Single(eventoRetornado.Inscricoes);

            Assert.Equal(evento.Id, eventoRetornado.Id);

            Assert.Equal(
                inscricao.Id,
                eventoRetornado.Inscricoes.First().Id );


        }


        [Fact]
        [Trait("Evento", "Repository")]
        public async Task ListarEventosAtivosRepositoryTeste()
        {
            // Arrange

            var context = CreateContext(DatabaseType.InMemory);

            var evento = _fix.Build<Evento>()
                .Without(x => x.Inscricoes)
                .Create();


            await context.Evento.AddAsync(evento);
            await context.SaveChangesAsync();


            context.ChangeTracker.Clear();

            var repo = new EventosRepositorio(context);

            //Act

            var result = await repo.GetEventosAtivos();


            // Assert

            Assert.NotNull(result);
            Assert.Single(result);

            var eventoRetornado = result.First();

            Assert.Equal(evento.Id, eventoRetornado.Id);
            Assert.Equal(true, eventoRetornado.Ativo);           


        }


        [Fact]
        [Trait("Evento","Repository")]
        public async Task GetEventoNomeRepositoryTeste()
        {

            //Arrange 
            var evento = _fix.Build<Evento>()
                .Without(x => x.Inscricoes)
                .Create();


            var context = CreateContext(DatabaseType.InMemory);

            await context.Evento.AddAsync(evento);
            await context.SaveChangesAsync();


            var repo = new EventosRepositorio(context);

            // Act
            var result = await repo.GetEventoNome(evento.Nome);


            //Assert

            Assert.NotNull(result);
            Assert.Equal(evento.Id, result.Id);
            Assert.Equal(evento.Nome, result.Nome);



        }

        [Fact]
        [Trait("Evento", "Repository")]
        public async Task GetEventoNome_DeveRetornarNull_QuandoEventoNaoExistir()
        {
            // Arrange
            var context = CreateContext(DatabaseType.InMemory);
            var repo = new EventosRepositorio(context);

            // Act
            var result = await repo.GetEventoNome("evento-inexistente");

            // Assert
            Assert.Null(result);
        }



        [Fact]
        [Trait("Evento","Repository")]
        public async Task UpdateEventoADMRepositoryTeste()
        {

            // Arrange

            double preco = 2.0;
            int quantidade = 20;

            var evento = _fix.Build<Evento>()
                .Without(x => x.Inscricoes)
                .Create();


            var context = CreateContext(DatabaseType.InMemory);

            await context.Evento.AddAsync(evento);
            await context.SaveChangesAsync();


            var repo = new EventosRepositorio(context);

            // Act 
            // result retorna bool
            var result = await repo.UpdateADM(evento.Id, preco, quantidade);


            // Assert

            Assert.NotNull(result);
            Assert.True(result);


            context.ChangeTracker.Clear();

            var eventoAtualizado = await context.Evento
                .FirstAsync(x => x.Id == evento.Id);

            Assert.Equal(preco, eventoAtualizado.Preco);
            Assert.Equal(
                quantidade,
                eventoAtualizado.QuantidadeDeKitsDisponiveis);
        }


        [Fact]
        [Trait("Evento", "Repository")]
        public async Task UpdateEventoADM_DeveRetornarFalse_QuandoEventoNaoExistir()
        {
            // Arrange
            var context = CreateContext(DatabaseType.InMemory);
            var repo = new EventosRepositorio(context);

            // Act
            var result = await repo.UpdateADM(
                999,
                2.0,
                20);

            // Assert
            Assert.False(result);
        }






        [Fact]
        [Trait("Evento","Repository")]
        public async Task FazerInscricaoEventoRepository()
        {
            // Arrange           

            var evento = _fix.Build<Evento>()
                .Without(x => x.Inscricoes)
                .Create();

            var quantidadeAnterior = evento.QuantidadeDeKitsDisponiveis;

            var ins = _fix.Build<Inscricao>()
                .Without(x => x.Usuario)
                .Without(x => x.Evento)
                .Create();         

            var context = CreateContext(DatabaseType.InMemory);

            await context.Evento.AddAsync(evento);
            await context.SaveChangesAsync();

            var repo = new EventosRepositorio(context);

            // Act
            var result = await repo.FazerInscricao(evento.Id, ins);

            // Assert

            Assert.True(result);


            context.ChangeTracker.Clear();

            var inscricaoSalva = await context.Inscricao
                .FirstOrDefaultAsync(x => x.Id == ins.Id);

            var eventoAtualizado = await context.Evento
                .FirstAsync(x => x.Id == evento.Id);

            Assert.NotNull(inscricaoSalva);
            Assert.NotEqual(default, inscricaoSalva.DataDeInscricao);

            Assert.Equal(
                quantidadeAnterior - 1,
                eventoAtualizado.QuantidadeDeKitsDisponiveis);
        

        }


        [Fact]
        [Trait("Evento", "Repository")]
        public async Task FazerInscricao_DeveRetornarFalse_QuandoEventoNaoExistir()
        {
            // Arrange
            var ins = _fix.Build<Inscricao>()
                .Without(x => x.Usuario)
                .Without(x => x.Evento)
                .Create();

            var context = CreateContext(DatabaseType.InMemory);
            var repo = new EventosRepositorio(context);

            // Act
            var result = await repo.FazerInscricao(999, ins);

            // Assert
            Assert.False(result);
        }


        [Fact]
        [Trait("Evento", "Repository")]
        public async Task FazerInscricao_DeveRetornarFalse_QuandoInscricaoJaExistir()
        {
            // Arrange
            var evento = _fix.Build<Evento>()
                .Without(x => x.Inscricoes)
                .Create();

            var ins = _fix.Build<Inscricao>()
                .Without(x => x.Usuario)
                .Without(x => x.Evento)
                .With(x => x.EventoId, evento.Id)
                .Create();

            var context = CreateContext(DatabaseType.InMemory);

            await context.Evento.AddAsync(evento);
            await context.Inscricao.AddAsync(ins);
            await context.SaveChangesAsync();

            var repo = new EventosRepositorio(context);

            var novaInscricao = _fix.Build<Inscricao>()
                .Without(x => x.Usuario)
                .Without(x => x.Evento)
                .With(x => x.EventoId, evento.Id)
                .With(x => x.UsuarioId, ins.UsuarioId)
                .Create();

            // Act
            var result = await repo.FazerInscricao(
                evento.Id,
                novaInscricao);

            // Assert
            Assert.False(result);
        }




    }
}
