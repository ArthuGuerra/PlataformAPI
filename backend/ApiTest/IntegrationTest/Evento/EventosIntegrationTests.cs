//using Application.DataTransferObject;
//using Domain.Entities;
//using Infraestrutura.BancoContexto;
//using Microsoft.AspNetCore.Mvc.Testing;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using System.Net;
//using System.Net.Http.Json;

//namespace ApiTest.IntegrationTestes.Eventos;

//public class EventosIntegrationTests
//    : IClassFixture<CustomWebApplicationFactory>
//{
//    private readonly CustomWebApplicationFactory _factory;
//    private readonly HttpClient _client;

//    public EventosIntegrationTests(
//        CustomWebApplicationFactory factory)
//    {
//        _factory = factory;
//        _client = factory.CreateClient();
//    }

//    [Fact]
//    [Trait("Evento","IntegracaoTeste")]
//    public async Task Deve_Listar_Eventos_Cadastrados()
//    {
//        // Arrange
//        using (var scope = _factory.Services.CreateScope())
//        {
//            var context = scope.ServiceProvider
//                .GetRequiredService<ApiContext>();

//            context.Evento.AddRange(
//                new Evento
//                {
//                    Nome = "Evento de Tecnologia",
//                    Descricao = "Evento sobre tecnologia",
//                    LocalEvento = "São Paulo",
//                    Imagem = "tecnologia.png",
//                    Ativo = true
//                },
//                new Evento
//                {
//                    Nome = "Evento Esportivo",
//                    Descricao = "Evento esportivo",
//                    LocalEvento = "Rio de Janeiro",
//                    Imagem = "esportivo.png",
//                    Ativo = true
//                }
//            );

//            await context.SaveChangesAsync();
//        }

//        // Act
//        var response = await _client.GetAsync("/api/eventos");

//        // Assert
//        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

//        var eventos = await response.Content
//            .ReadFromJsonAsync<List<EventoDTO>>();

//        Assert.NotNull(eventos);
//        Assert.Equal(2, eventos.Count);

//        Assert.Contains(
//            eventos,
//            evento => evento.Nome == "Evento de Tecnologia");

//        Assert.Contains(
//            eventos,
//            evento => evento.Nome == "Evento Esportivo");
//    }
//}