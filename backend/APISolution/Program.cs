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
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using APISolution.MyRateLimit;
using Asp.Versioning;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler=ReferenceHandler.IgnoreCycles);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi




builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7214",
            "https://meusite.com",
            "https://apirequest.io") 
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});



builder.Services.AddApiVersioning(v =>
{
    v.DefaultApiVersion = new ApiVersion(1, 0);
    v.AssumeDefaultVersionWhenUnspecified = true;
    v.ReportApiVersions = true;
    v.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader());

}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});







var myOptions = new MyRateLimitOptions();

builder.Configuration.GetSection(MyRateLimitOptions.MyRateLimit).Bind(myOptions);



// rate limite global
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext => RateLimitPartition.GetFixedWindowLimiter(
                                        partitionKey: httpcontext.User.Identity?.Name ??
                                        httpcontext.Request.Headers.Host.ToString(),
                    factory: partion => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = myOptions.AutoReplenishment,
                        PermitLimit = myOptions.PermitLimit,
                        QueueLimit = myOptions.QueueLimit,
                        Window = TimeSpan.FromSeconds(myOptions.Window)
                    }));
});






string connectionString = "Server=localhost\\SQLEXPRESS;Database=ApiContextTeste;Trusted_Connection=True;TrustServerCertificate=True";

string SqlServer = builder.Configuration.GetConnectionString("ConexaoPadrao");

builder.Services.AddDbContext<ApiContext>(options => options.UseSqlServer(connectionString));





builder.Services.AddIdentity<Usuario, IdentityRole>().AddEntityFrameworkStores<ApiContext>().AddDefaultTokenProviders();







builder.Services.AddScoped<ApiContext>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repositorio<>));
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IUsuariosRepository,UsuariosRepositorio>();
builder.Services.AddScoped<IEventosRepository,EventosRepositorio>();
builder.Services.AddScoped<IEventoServices,EventosServices>();
builder.Services.AddScoped<IUsuarioServices,UsuariosServices>();
builder.Services.AddScoped<IInscricaoRepository, InscricaoRepository>();
builder.Services.AddScoped<IInscricaoServices, InscricaoServices>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddAutoMapper(cfg => { },
    typeof(DomainDTOMappingProfile));





builder.Services.AddScoped<ApiLoggingFilter>();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiLoggingFilter>();
});



Log.Logger = new LoggerConfiguration().WriteTo.File("C:\\Logs\\log.txt", rollingInterval: RollingInterval.Day).CreateLogger();

builder.Host.UseSerilog();





var secretKey = builder.Configuration["JWT:SecretKey"] ?? throw new ArgumentException("Invalid secret key!!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey
            (
                Encoding.UTF8.GetBytes(secretKey)
            )
    };
});




builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin", "SuperAdmin"));
    options.AddPolicy("Super", policy => policy.RequireRole("SuperAdmin").RequireClaim(ClaimTypes.Name,"ArthurGuerra","AishaGerage").RequireClaim(ClaimTypes.NameIdentifier, "6e509426-cded-49da-bbf6-a7878f6930d5", ""));
    options.AddPolicy("User", policy =>
    policy.RequireRole("User", "Admin", "SuperAdmin"));
});











builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiSolution", Version = "v1" });

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "APISolution",
        Description = "Api para plataforma de corrida e para os orgãos da ADRI, APCI e LDIF",
        TermsOfService = new Uri("https://www.youtube.com/@Dev-Guerra"),
        Contact = new OpenApiContact
        {
            Name = "Arthur Guerra & Aisha Gerage",
            Email = "agtech@gmail.com",
            Url = new Uri("https://www.youtube.com/@Dev-Guerra")
        },
        License = new OpenApiLicense
        {
            Name = "Usar sobre LICX",
            Url = new Uri("https://www.youtube.com/@Dev-Guerra"),
        }
    });



    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT",
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});















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
app.UseStaticFiles();

app.UseRouting();
app.UseCors("ApiPolicy");
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();