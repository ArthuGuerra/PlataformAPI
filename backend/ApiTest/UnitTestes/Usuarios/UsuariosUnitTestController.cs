using APISolution.Controllers;
using Application.DataTransferObject;
using Application.DataTransferObject.IdentityDTO;
using Application.Interfaces;
using AutoFixture;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTest.UnitTestes.Usuarios
{
    public class UsuariosUnitTestController : TestBase
    {
        private readonly Fixture _fix;

        public UsuariosUnitTestController()
        {
            _fix = new Fixture();
        }


        [Fact]
        [Trait("Usuario","Controller")]
        public async Task ListarUsuarioControllerTeste()
        {
            // Arrange
            var usuarios = _fix.Build<UsuarioPrintDTO>()
                .CreateMany(10).ToList();


            var services = new Mock<IUsuarioServices>();

            services.Setup(r => r.GetAllUsers()).ReturnsAsync(usuarios);

            var controller = new UsuarioController(services.Object);


            // Act

            var result = await controller.GetAll();

            // Assert

            services.Verify(r => r.GetAllUsers(), Times.Once());

            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);

            var model = Assert.IsAssignableFrom<IEnumerable<UsuarioPrintDTO>>(okResult.Value);

            Assert.Equal(usuarios, okResult.Value); 


        }




        [Fact]
        [Trait("Usuario", "Controller")]
        public async Task ListarInscricoesUsuarioControllerTeste()
        {
            // Arrange
            var usuarios = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .CreateMany(10).ToList();


            var services = new Mock<IUsuarioServices>();

            services.Setup(r => r.UsuarioInscricao()).ReturnsAsync(usuarios);

            var controller = new UsuarioController(services.Object);


            // Act

            var result = await controller.UsuarioInscricoes();

            // Assert

            services.Verify(r => r.UsuarioInscricao(), Times.Once());

            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            Assert.Equal(200, okResult.StatusCode);

            var model = Assert.IsAssignableFrom<IEnumerable<Usuario>>(okResult.Value);

            Assert.Equal(usuarios, model);


        }





        [Fact]
        [Trait("Usuario","Controller")]
        public async Task GetIdUsuarioTesteController()
        {
            // Arrange
            string id = "10";

            var usuario = _fix.Build<UsuarioPrintDTO>().Create();

            
            var services = new Mock<IUsuarioServices>();

            services.Setup(r => r.GetIdUsuario(id)).ReturnsAsync(usuario);


            var controller = new UsuarioController(services.Object);

            //Act
            var result = await controller.GetId(id);


            // Assert
            services.Verify(r => r.GetIdUsuario(id), Times.Once);

            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal(usuario, okResult.Value);


        }



        [Fact]
        [Trait("Usuario","Controller")]
        public async Task GetNomeUsuarioControllerTeste()
        {
            var usuario = _fix.Build<UsuarioPrintDTO>().Create();


            var services = new Mock<IUsuarioServices>();

            services.Setup(r => r.GetNomeUsers(usuario.UserName)).ReturnsAsync(usuario);


            var controller = new UsuarioController(services.Object);


            // Act

            var result = await controller.GetNome(usuario.UserName);


            //Assert 

            services.Verify(r => r.GetNomeUsers(usuario.UserName), Times.Once);

            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal(usuario, okResult.Value);


        }



        [Fact]
        [Trait("Usuario","Controller")]
        public async Task UpdateUsuarioControllerTeste()
        {
            // Arrange
            string id = "10";

            var usuario = _fix.Build<UsuarioPrintDTO>().Create();

            var services = new Mock<IUsuarioServices>();


            services.Setup(r => r.UpdateUsers(id, usuario)).ReturnsAsync(true);


            var controller = new UsuarioController(services.Object);


            // Act
            var result = await controller.UpdateUser(id, usuario);


            // Assert

            services.Verify(r => r.UpdateUsers(id, usuario), Times.Once);

            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.True((bool)okResult.Value);


        }


        [Fact]
        [Trait("Usuario","Controller")]
        public async Task DeleteUsuarioControllerTeste()
        {
            // Arrange
            string id = "10";       

            var services = new Mock<IUsuarioServices>();


            services.Setup(r => r.DeleteUsers(id)).ReturnsAsync(true);


            var controller = new UsuarioController(services.Object);


            // Act
            var result = await controller.DeleteUser(id);


            // Assert

            services.Verify(r => r.DeleteUsers(id), Times.Once);

            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.True((bool)okResult.Value);


        }



        [Fact]
        [Trait("Usuario","Controller")]
        public async Task MudarSenhaUsuarioControllerTeste()
        {
            // Arrange           

            string email = "emaildeteste@gmail.com";

            var usuario = _fix.Build<UsuarioSenhaDTO>().Create();

            var services = new Mock<IUsuarioServices>();


            services.Setup(r => r.UpdateSenha(email, usuario)).ReturnsAsync(true);


            var controller = new UsuarioController(services.Object);


            // Act
            var result = await controller.ChangePass(email, usuario);


            // Assert

            services.Verify(r => r.UpdateSenha(email, usuario), Times.Once);

            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);

            Assert.Equal("Senha Atualizada com sucesso", okResult.Value);
        }


        [Fact]
        [Trait("Usuario","Controller")]
        public async Task MostrarRolesUsuarioControllerTeste()
        {
            // Arrange
            var usuarios = _fix.Build<UserRolesDTO>()                         
                .CreateMany(10).ToList();

            var services = new Mock<IUsuarioServices>();

            services.Setup(r => r.ShowUsersRoles()).ReturnsAsync(usuarios);

            var controller = new UsuarioController(services.Object);

            // Act

            var result = await controller.ShowUserRole();

            // Assert

            services.Verify(r => r.ShowUsersRoles(), Times.Once);

            Assert.NotNull(result);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(200, okResult.StatusCode);            

            var model = Assert.IsAssignableFrom<IEnumerable<UserRolesDTO>>(okResult.Value);

            Assert.Equal(usuarios, model);
        }


    }
}
