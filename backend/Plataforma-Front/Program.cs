using Microsoft.Extensions.DependencyInjection;
using Plataforma_Front.Interfaces;
using Plataforma_Front.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("APISolution", a =>
{
    a.BaseAddress = new Uri(builder.Configuration["ServiceUri:APISolution"]!);
});



builder.Services.AddScoped<IEventosServicesMVC, EventosServicesMVC>();
builder.Services.AddScoped<IInscricaoServiceMVC, InscricaoServiceMVC>();





var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Evento/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Evento}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
