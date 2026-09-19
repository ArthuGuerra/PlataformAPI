using AutoFixture;
using Domain.Entities;
using Infraestrutura.ContextRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTest.UnitTestes.Inscricoes
{
    public class InscricaoUnitTesteRepositoryGenerics : TestBase
    {
        private readonly Fixture _fix;

        public InscricaoUnitTesteRepositoryGenerics()
        {
            _fix = new Fixture();
        }

        [Fact]
        [Trait("Inscricao","RepositoryGenerics")]
        public async Task GetAllAsyncInscricaoRepositoryTeste()
        {
            // Arrange

            var context = CreateContext(DatabaseType.InMemory);

            var inscricao = _fix.Build<Inscricao>()
                .Without(x => x.Evento)
                .Without(x => x.Usuario)
                .CreateMany(5).ToList();


            await context.Inscricao.AddRangeAsync(inscricao);
            await context.SaveChangesAsync();


            var repo = new InscricaoRepository(context);

            // Act

            var result = await repo.GetAllAsync();

            // Assert

            Assert.NotNull(result);
            Assert.Equal(5, result.Count);

        }
        


        [Fact]
        [Trait("Inscricao", "RepositoryGenerics")]
        public async Task GetIdInscricaoRepositoryTeste()
        {
            // Arrange

            var context = CreateContext(DatabaseType.InMemory);

            var inscricao = _fix.Build<Inscricao>()
                .Without(x => x.Evento)
                .Without(x => x.Usuario)
                .Create();


            await context.Inscricao.AddAsync(inscricao);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();


            var repo = new InscricaoRepository(context);

            // Act

            var result = await repo.GetIdAsync(inscricao.Id);

            // Assert

            Assert.NotNull(result);
            Assert.Equal(inscricao.Id, result.Id);
        }


        [Fact]
        [Trait("Inscricao", "RepositoryGenerics")]
        public async Task GetIdAsync_DeveRetornarNull_QuandoIdNaoExistir()
        {
            // Arrange
            var context = CreateContext(DatabaseType.InMemory);
            var repo = new InscricaoRepository(context);

            // Act
            var result = await repo.GetIdAsync(999999);

            // Assert
            Assert.Null(result);
        }




        [Fact]
        [Trait("Inscricao", "RepositoryGenerics")]
        public async Task CreateInscricaoRepositoryGenericsTeste()
        {
            // Arrange

            var context = CreateContext(DatabaseType.InMemory);

            var inscricao = _fix.Build<Inscricao>()
                .Without(x => x.Evento)
                .Without(x => x.Usuario)
                .Create();            


            var repo = new InscricaoRepository(context);

            // Act

            var result = repo.Create(inscricao);

            await context.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(inscricao.Id, result.Id);

            var salvo = await context.Inscricao.FirstOrDefaultAsync(x => x.Id == inscricao.Id);

            Assert.NotNull(salvo);
            Assert.Equal(inscricao.Id, salvo.Id);
            Assert.Equal(inscricao.NomeEvento, salvo.NomeEvento);

        }






        [Fact]
        [Trait("Inscricao","RepositoryGenerics")]
        public async Task UpdateinscricaoRepositoryGenericsTeste()
        {
            // Arrange

            var context = CreateContext(DatabaseType.InMemory);

            var inscricao = _fix.Build<Inscricao>()
                .Without(x => x.Evento)
                .Without(x => x.Usuario)
                .Create();


            await context.Inscricao.AddAsync(inscricao);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            string novoNome = "Nome Atualizado!";

            inscricao.NomeEvento = novoNome;

            var repo = new InscricaoRepository(context);
          

            // Act

            var result = repo.Update(inscricao);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();


            // Assert
            Assert.NotNull(result);
            Assert.Equal(inscricao.Id, result.Id);

            context.ChangeTracker.Clear();

            var salvo = await context.Inscricao.FirstOrDefaultAsync(x => x.Id == inscricao.Id);

            Assert.NotNull(salvo);
            Assert.Equal(novoNome, salvo.NomeEvento);
        }



        [Fact]
        [Trait("Inscricao", "RepositoryGenerics")]
        public async Task DeleteInscricaoRepositoryGenericsTeste()
        {
            // Arrange

            var context = CreateContext(DatabaseType.InMemory);

            var inscricao = _fix.Build<Inscricao>()
                .Without(x => x.Evento)
                .Without(x => x.Usuario)
                .Create();


            await context.Inscricao.AddAsync(inscricao);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var repo = new InscricaoRepository(context);
         

            // Act

            var result = repo.Delete(inscricao);

            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();


            // Assert
            Assert.NotNull(result);
            Assert.Equal(inscricao.Id, result.Id);

            context.ChangeTracker.Clear();

            var salvo = await context.Inscricao.FirstOrDefaultAsync(x => x.Id == inscricao.Id);

            Assert.Null(salvo);           
        }
    }
}
