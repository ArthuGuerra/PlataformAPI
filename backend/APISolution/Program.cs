using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infraestrutura.BancoContexto;
using System.Security.Cryptography.Xml;
using System.Text.Json.Serialization;
using APISolution.MiddlawareException;
using Infraestrutura.Interfaces;
using Infraestrutura.ContextRepository;
using APISolution.Filter;
using Application.Interfaces;
using Application.Services;
using Application.DataTransferObject;
using Application.MapperExtension;
using AutoMapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi







string connectionString = "Server=localhost\\SQLEXPRESS;Database=ApiContextTeste;Trusted_Connection=True;TrustServerCertificate=True";

string SqlServer = builder.Configuration.GetConnectionString("ConexaoPadrao");

builder.Services.AddDbContext<ApiContext>(options => options.UseSqlServer(connectionString));









builder.Services.AddScoped<ApiContext>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repositorio<>));
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IUsuariosRepository,UsuariosRepositorio>();
builder.Services.AddScoped<IEventosRepository,EventosRepositorio>();
builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<IEventoServices,EventosServices>();
builder.Services.AddScoped<IUsuarioServices,UsuariosServices>();
builder.Services.AddScoped<IInscricaoRepository, InscricaoRepository>();
builder.Services.AddScoped<IInscricaoServices, InscricaoServices>();

builder.Services.AddAutoMapper(cfg => { },
    typeof(DomainDTOMappingProfile));








builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionMiddlewareExtensions();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("swagger/v1/swagger.json", "APISolution");
    });
}

app.UseHttpsRedirection();

//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();