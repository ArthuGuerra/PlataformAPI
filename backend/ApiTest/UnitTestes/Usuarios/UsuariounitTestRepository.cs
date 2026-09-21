using AutoFixture;
using Domain.Entities;
using Infraestrutura.ContextRepository;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTest.UnitTestes.Usuarios
{
    public class UsuariounitTestRepository : TestBase
    {
        private readonly Fixture _fix;

        public UsuariounitTestRepository()
        {
            _fix = new Fixture();
        }

        [Fact]
        [Trait("Usuario","Repository")]
        public async Task GetAllAsyncUsuarioRepository()
        {
            // arrange
            var users = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .CreateMany(5).ToList();

            var context = CreateContext(DatabaseType.InMemory);

            await context.Usuario.AddRangeAsync(users);
            await context.SaveChangesAsync();


            var repo = new UsuariosRepositorio(context);

            // Act

            var result = await repo.GetAllAsync();

            // Assert

            Assert.NotNull(result);
            Assert.Equal(5, result.Count);

            Assert.All( users, user => Assert.Contains( result, retorno => retorno.Id == user.Id));

        }


        [Fact]
        [Trait("Usuario", "Repository")]
        public async Task GetAllAtivosAsync_DeveRetornarTodosOsUsuariosAtivos()
        {
            // Arrange
            var users = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .With(x => x.Ativo, true)
                .CreateMany(5)
                .ToList();

            var context = CreateContext(DatabaseType.InMemory);

            await context.Usuario.AddRangeAsync(users);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var repo = new UsuariosRepositorio(context);

            // Act
            var result = await repo.GetAllAtivosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Count);

            Assert.All(
                result,
                user => Assert.True(user.Ativo));

            Assert.All(
                users,
                user => Assert.Contains(
                    result,
                    retorno => retorno.Id == user.Id));
        }



        [Fact]
        [Trait("Usuario", "Repository")]
        public async Task GetUsuarioInscricaoRepository()
        {
            // Arrange

            var context = CreateContext(DatabaseType.InMemory);

            var usuario = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .Create();

            var inscricao = _fix.Build<Inscricao>()
                .Without(x => x.Evento)
                .Without(x => x.Usuario)
                .Create();

            usuario.Inscricoes = new List<Inscricao>
            {
                inscricao
            };


            await context.Usuario.AddAsync(usuario);
            await context.SaveChangesAsync();


            context.ChangeTracker.Clear();

            var repo = new UsuariosRepositorio(context);

            //Act

            var result = await repo.GetUsuarioInscricao();


            // Assert

            Assert.NotNull(result);
            Assert.Single(result);

            var userRetornado = result.First();

            Assert.NotNull(userRetornado.Inscricoes);
            Assert.Single(userRetornado.Inscricoes);

            Assert.Equal(usuario.Id, userRetornado.Id);
            Assert.Equal(usuario.UserName, userRetornado.UserName);
            Assert.Equal(usuario.Email, userRetornado.Email);

            Assert.Equal(
                inscricao.Id,
                userRetornado.Inscricoes.First().Id);

        }
    }
}
