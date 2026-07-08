using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Plataforma_Front.Interfaces;
using Plataforma_Front.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddHttpClient("APISolution", a =>
{
    a.BaseAddress = new Uri(builder.Configuration["ServiceUri:APISolution"]!);
});


builder.Services.AddHttpClient("APIAuth", a =>
{
    a.BaseAddress = new Uri(builder.Configuration["ServiceUri:APIAuth"]!);
    a.DefaultRequestHeaders.Accept.Clear();
    a.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});







builder.Services.AddScoped<IEventosServicesMVC, EventosServicesMVC>();
builder.Services.AddScoped<IInscricaoServiceMVC, InscricaoServiceMVC>();
builder.Services.AddScoped<IAuthenticacao, AuthenticacaoServiceMVC>();

builder.Services
    .AddAuthentication("Cookies")
    .AddCookie("Cookies");











var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Evento/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Evento}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
