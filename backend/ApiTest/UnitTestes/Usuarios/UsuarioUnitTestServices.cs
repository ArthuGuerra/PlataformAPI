using Application.DataTransferObject;
using Application.DataTransferObject.IdentityDTO;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Infraestrutura.Interfaces;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTest.UnitTestes.Usuarios
{
    public class UsuarioUnitTestServices : TestBase
    {
        private readonly Fixture _fix;

        public UsuarioUnitTestServices()
        {
            _fix = new Fixture();
        }


        [Fact]
        [Trait("Usuario","Services")]
        public async Task GetUsuariosListaTeste()
        {
            // Arrange
            var users = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .CreateMany(20).ToList();


            var userRepo = new Mock<IUnitOfWork>();
            var userStore = new Mock<IUserStore<Usuario>>();

            var userMan = new Mock<UserManager<Usuario>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            var role = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                null,
                null,
                null,
                null
            );

            userRepo.Setup(r => r.UsuariosRepository.GetAllAsync()).ReturnsAsync(users);

            var services = new UsuariosServices(userRepo.Object, _mapper, userMan.Object, role.Object);


            // Act

            var result = await services.GetAllUsers();

            ///Assert

            userRepo.Verify(r => r.UsuariosRepository.GetAllAsync(), Times.Once);
            Assert.Equal(users.Count, result.Count);
        }



        [Fact]
        [Trait("Usuario", "Services")]
        public async Task GetUsuariosAtivosListaTeste()
        {
            // Arrange
            var users = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .CreateMany(20).ToList();


            var userRepo = new Mock<IUnitOfWork>();
            var userStore = new Mock<IUserStore<Usuario>>();

            var userMan = new Mock<UserManager<Usuario>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            var role = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                null,
                null,
                null,
                null
            );

            userRepo.Setup(r => r.UsuariosRepository.GetAllAtivosAsync()).ReturnsAsync(users);

            var services = new UsuariosServices(userRepo.Object, _mapper, userMan.Object, role.Object);


            // Act

            var result = await services.GetAtivos();

            ///Assert

            userRepo.Verify(r => r.UsuariosRepository.GetAllAtivosAsync(), Times.Once);
            Assert.Equal(users.Count, result.Count);
        }


        [Fact]
        [Trait("Usuario", "Services")]
        public async Task GetNomeUserTeste()
        {
            // Arrange
            var user = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .Create();


            var userRepo = new Mock<IUnitOfWork>();
            var userStore = new Mock<IUserStore<Usuario>>();

            var userMan = new Mock<UserManager<Usuario>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            var role = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                null,
                null,
                null,
                null
            );

            userMan.Setup(u => u.FindByNameAsync(user.UserName)).ReturnsAsync(user);

            var services = new UsuariosServices(userRepo.Object, _mapper, userMan.Object, role.Object);


            // Act

            var result = await services.GetNomeUsers(user.UserName);

            ///Assert

            userMan.Verify(r => r.FindByNameAsync(user.UserName), Times.Once);
            Assert.Equal(user.UserName,result.UserName);
        }


        [Fact]
        [Trait("Usuario", "Services")]
        public async Task GetUserByIdTeste()
        {
            // Arrange
            var user = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .Create();


            var userRepo = new Mock<IUnitOfWork>();
            var userStore = new Mock<IUserStore<Usuario>>();

            var userMan = new Mock<UserManager<Usuario>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            var role = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                null,
                null,
                null,
                null
            );

            userMan.Setup(u => u.FindByIdAsync(user.Id)).ReturnsAsync(user);

            var services = new UsuariosServices(userRepo.Object, _mapper, userMan.Object, role.Object);


            // Act

            var result = await services.GetIdUsuario(user.Id);

            ///Assert

            userMan.Verify(r => r.FindByIdAsync(user.Id), Times.Once);
            Assert.Equal(user.UserName, result.UserName);
        }



        [Fact]
        [Trait("Usuario", "Services")]
        public async Task UpdateUsuario_DeveAtualizarUsuario()
        {
            // Arrange
            var user = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .Create();

            var dto = new UsuarioPrintDTO
            {
                UserName = "usuario.atualizado",
                Email = "usuario.atualizado@email.com"
            };

            var userStore = new Mock<IUserStore<Usuario>>();

            var userManager = new Mock<UserManager<Usuario>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            var roleManager = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                null,
                null,
                null,
                null);

            var unitOfWork = new Mock<IUnitOfWork>();

            userManager
                .Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            // Não existe outro usuário usando o novo nome
            userManager
                .Setup(x => x.FindByNameAsync(dto.UserName))
                .ReturnsAsync((Usuario?)null);

            // Não existe outro usuário usando o novo e-mail
            userManager
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync((Usuario?)null);

            userManager
                .Setup(x => x.UpdateAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            var service = new UsuariosServices(
                unitOfWork.Object,
                _mapper,
                userManager.Object,
                roleManager.Object);

            // Act
            var result = await service.UpdateUsers(user.Id, dto);

            // Assert
            Assert.True(result);

            Assert.Equal(dto.UserName, user.UserName);
            Assert.Equal(dto.Email, user.Email);

            userManager.Verify(
                x => x.FindByIdAsync(user.Id),
                Times.Once);

            userManager.Verify(
                x => x.FindByNameAsync(dto.UserName),
                Times.Once);

            userManager.Verify(
                x => x.FindByEmailAsync(dto.Email),
                Times.Once);

            userManager.Verify(
                x => x.UpdateAsync(user),
                Times.Once);
        }



        [Fact]
        [Trait("Usuario", "Services")]
        public async Task UpdateUsuario_DeveRetornarFalse_QuandoEmailJaEstiverEmUso()
        {
            // Arrange
            var user = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .Create();

            var outroUsuario = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .Create();

            var dto = new UsuarioPrintDTO
            {
                UserName = "novo.usuario",
                Email = "email@existente.com"
            };

            var userStore = new Mock<IUserStore<Usuario>>();

            var userManager = new Mock<UserManager<Usuario>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);

            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            var roleManager = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                null,
                null,
                null,
                null);

            var unitOfWork = new Mock<IUnitOfWork>();

            userManager
                .Setup(x => x.FindByIdAsync(user.Id))
                .ReturnsAsync(user);

            userManager
                .Setup(x => x.FindByNameAsync(dto.UserName))
                .ReturnsAsync((Usuario?)null);

            userManager
                .Setup(x => x.FindByEmailAsync(dto.Email))
                .ReturnsAsync(outroUsuario);

            var service = new UsuariosServices(
                unitOfWork.Object,
                _mapper,
                userManager.Object,
                roleManager.Object);

            // Act
            var result = await service.UpdateUsers(user.Id, dto);

            // Assert
            Assert.False(result);

            userManager.Verify(
                x => x.UpdateAsync(It.IsAny<Usuario>()),
                Times.Never);
        }




        [Fact]
        [Trait("Usuario", "Services")]
        public async Task DeleteUserTeste()
        {
            // Arrange
            var user = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .Create();

            var map = _mapper.Map<UsuarioPrintDTO>(user);


            var userRepo = new Mock<IUnitOfWork>();
            var userStore = new Mock<IUserStore<Usuario>>();

            var userMan = new Mock<UserManager<Usuario>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            var role = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                null,
                null,
                null,
                null
            );

            userMan.Setup(u => u.FindByIdAsync(user.Id)).ReturnsAsync(user);

            user.Ativo = false;

            userMan.Setup(u => u.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);


            // Act

            var services = new UsuariosServices(userRepo.Object, _mapper, userMan.Object, role.Object);

            var result = await services.UpdateUsers(user.Id, map);



            // Assert

            userMan.Verify(r => r.FindByIdAsync(user.Id), Times.Once());

            userMan.Verify(r => r.UpdateAsync(user), Times.Once());

            Assert.True(result);

        }

        [Fact]
        [Trait("Usuario", "Services")]
        public async Task ShowUserRolesTeste()
        {
            // Arrange
            var user = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .Create();


            var users = _fix.Build<Usuario>()
              .Without(x => x.Inscricoes)
              .Without(x => x.InscricoesApp)
              .CreateMany(20).ToList();


            var roles = new List<string>
                {
                    "Administrador",
                    "Usuario"
                };




            var userRepo = new Mock<IUnitOfWork>();
            var userStore = new Mock<IUserStore<Usuario>>();

            var userMan = new Mock<UserManager<Usuario>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            var role = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                null,
                null,
                null,
                null
            );
           

            userRepo.Setup(r => r.UsuariosRepository.GetAllAsync()).ReturnsAsync(users);

            userMan.Setup(u => u.GetRolesAsync(It.IsAny<Usuario>())).ReturnsAsync(roles);
            
           

            var services = new UsuariosServices(userRepo.Object, _mapper, userMan.Object, role.Object);


            // Act

            var result = await services.ShowUsersRoles();


            // Assert            

            userRepo.Verify(r => r.UsuariosRepository.GetAllAsync(), Times.Once());
           

            userMan.Verify(r => r.GetRolesAsync(It.IsAny<Usuario>()), Times.Exactly(users.Count));

            Assert.Equal(users.Count, result.Count);

            foreach (var item in result)
            {
                Assert.Contains(item.Roles, x => x == "Administrador");
                Assert.Contains(item.Roles, x => x == "Usuario");
            }

        }


        [Fact]
        [Trait("Usuario", "Services")]
        public async Task UsuarioInscricaoTeste()
        {
            // Arrange
            var user = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .Create();

            var users = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .CreateMany(10).ToList();


            var userRepo = new Mock<IUnitOfWork>();
            var userStore = new Mock<IUserStore<Usuario>>();

            var userMan = new Mock<UserManager<Usuario>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            var role = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                null,
                null,
                null,
                null
            );
            

            userRepo.Setup(u => u.UsuariosRepository.GetUsuarioInscricao()).ReturnsAsync(users);


            // Act

            var services = new UsuariosServices(userRepo.Object, _mapper, userMan.Object, role.Object);

            var result = await services.UsuarioInscricao();


            // Assert

            userRepo.Verify(u => u.UsuariosRepository.GetUsuarioInscricao(),Times.Once);

            Assert.Equal(users.Count, result.Count);
        }


        [Fact]
        [Trait("Usuario", "Services")]
        public async Task UpdateSenhaTeste()
        {
            // Arrange
            var user = _fix.Build<Usuario>()
                .Without(x => x.Inscricoes)
                .Without(x => x.InscricoesApp)
                .Create();

            var pass = _fix.Build<UsuarioSenhaDTO>()                
                .Create();


            var userRepo = new Mock<IUnitOfWork>();
            var userStore = new Mock<IUserStore<Usuario>>();

            var userMan = new Mock<UserManager<Usuario>>(
                userStore.Object,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            var roleStore = new Mock<IRoleStore<IdentityRole>>();

            var role = new Mock<RoleManager<IdentityRole>>(
                roleStore.Object,
                null,
                null,
                null,
                null
            );


            userMan.Setup(u => u.FindByEmailAsync(user.Email)).ReturnsAsync(user);

            userMan.Setup(u => u.ChangePasswordAsync(user, pass.SenhaAtual, pass.NewSenha)).ReturnsAsync(IdentityResult.Success);



            // Act

            var services = new UsuariosServices(userRepo.Object, _mapper, userMan.Object, role.Object);

            var result = await services.UpdateSenha(user.Email, pass);


            // Assert

            userMan.Verify(u => u.FindByEmailAsync(user.Email),Times.Once);

            userMan.Verify(u => u.ChangePasswordAsync(user, pass.SenhaAtual, pass.NewSenha), Times.Once);

            Assert.True(result);



        }


    }
}
