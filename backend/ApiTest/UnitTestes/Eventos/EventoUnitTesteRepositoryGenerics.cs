using AutoFixture;
using Domain.Entities;
using Infraestrutura.ContextRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTest.UnitTestes.Eventos
{
    public class EventoUnitTesteRepositoryGenerics : TestBase
    {
        private readonly Fixture _fix;

        public EventoUnitTesteRepositoryGenerics()
        {
            _fix = new Fixture();
        }



        [Fact]
        [Trait("Evento", "RepositoryGenerico")]
        public async Task GetAllAsync_DeveRetornarEntidades()
        {
            // Arrange

            var context = CreateContext(DatabaseType.InMemory);

            var eventos = _fix.Build<Evento>()
                .Without(x => x.Inscricoes)
                .CreateMany(10).ToList();

            await context.Evento.AddRangeAsync(eventos);
            await context.SaveChangesAsync();

            var repo = new EventosRepositorio(context);

            // Act

            var result = await repo.GetAllAsync();

            Assert.NotNull(result);
            Assert.Equal(10, result.Count);

        }



        [Fact]
        [Trait("Evento", "RepositoryGenerico")]
        public async Task GetByIdAsync_DeveRetornarEntidade()
        {
            // Arrange
            var evento = _fix.Build<Evento>()
                 .Without(x => x.Inscricoes)
                 .Create();

            var context = CreateContext(DatabaseType.InMemory);

            await context.Evento.AddAsync(evento);
            await context.SaveChangesAsync();


            // Limpa o rastreamento para realizar uma nova consulta
            context.ChangeTracker.Clear();

            var repo = new EventosRepositorio(context);

            /// Act
            var result = await repo.GetIdAsync(evento.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(evento.Id, result.Id);

        }




        [Fact]
        [Trait("Evento", "RepositoryGenerico")]
        public async Task CreateAsync_DeveSalvarEntidade()
        {
            // Arrange
            var evento = _fix.Build<Evento>()
                 .Without(x => x.Inscricoes)
                 .Create();

            var context = CreateContext(DatabaseType.InMemory);            

            var repo = new EventosRepositorio(context);

            // Act

            var result = repo.Create(evento);

            await context.SaveChangesAsync();


            // Assert
            Assert.NotNull(result);
            Assert.Equal(evento.Id, result.Id);

            var eventoSalvo = await context.Evento.FirstOrDefaultAsync(x => x.Id == evento.Id);

            Assert.NotNull(eventoSalvo);
            Assert.Equal(evento.Id, eventoSalvo.Id);

        }




        [Fact]
        [Trait("Evento", "RepositoryGenerico")]
        public async Task UpdateAsync_DeveAtualizarEntidade()
        {
            // Arrange
            var evento = _fix.Build<Evento>()
                 .Without(x => x.Inscricoes)
                 .Create();

            var context = CreateContext(DatabaseType.InMemory);

            await context.Evento.AddAsync(evento);
            await context.SaveChangesAsync();


            context.ChangeTracker.Clear();

            var novoNome = "Evento atualizado";

            evento.Nome = novoNome;


            var repo = new EventosRepositorio(context);

            // Act

            var result = repo.Update(evento);

            await context.SaveChangesAsync();


            // Assert
            Assert.NotNull(result);
            Assert.Equal(evento.Id, result.Id);

            context.ChangeTracker.Clear();

            var eventoAtualizado = await context.Evento
                .FirstOrDefaultAsync(x => x.Id == evento.Id);

            Assert.NotNull(eventoAtualizado);
            Assert.Equal(novoNome, eventoAtualizado.Nome);

        }




        [Fact]
        [Trait("Evento", "RepositoryGenerico")]
        public async Task DeleteAsync_DeveRemoverEntidade()
        {
            // Arrange
            var evento = _fix.Build<Evento>()
                 .Without(x => x.Inscricoes)
                 .Create();

            var context = CreateContext(DatabaseType.InMemory);

            await context.Evento.AddAsync(evento);
            await context.SaveChangesAsync();


            var repo = new EventosRepositorio(context);

            // Act

            var result = repo.Delete(evento);

            await context.SaveChangesAsync();



            // Assert
            Assert.NotNull(result);
            Assert.Equal(evento.Id, result.Id);

            var eventoRemovido = await context.Evento
                .FirstOrDefaultAsync(x => x.Id == evento.Id);

            Assert.Null(eventoRemovido);

        }
    }
}
