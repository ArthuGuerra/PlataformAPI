using APISolution.Filter;
using APISolution.MiddlawareException;
using APISolution.MyRateLimit;
using Application.Configuration;
using Application.Configuration;
using Application.DataTransferObject;
using Application.Interfaces;
using Application.InterfacesApp;
using Application.MapperExtension;
using Application.Services;
using Application.ServicesApp;
using Asp.Versioning;
using AutoMapper;
using Domain.Entities;
using Infraestrutura.BancoContexto;
using Infraestrutura.ContextRepository;
using Infraestrutura.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();




static string RequiredConfiguration(IConfiguration configuration, string key)
{
    var value = configuration[key];

    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException(
            $"Configuração obrigatória ausente: {key}");
    }

    return value;
}

var connectionString = RequiredConfiguration(builder.Configuration,
    "ConnectionStrings:ConexaoPadrao");

var superAdminUserId = RequiredConfiguration(
    builder.Configuration,
    "SuperAdmin:UserId");





builder.Services.AddDbContext<ApiContext>(options =>
    options.UseSqlServer(connectionString));





var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()
    ?? Array.Empty<string>();

if (allowedOrigins.Length == 0)
{
    throw new InvalidOperationException(
        "Nenhuma origem CORS foi configurada.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
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


if (myOptions.PermitLimit <= 0)
{
    throw new InvalidOperationException(
        "MyRateLimit:PermitLimit deve ser maior que zero.");
}

if (myOptions.Window <= 0)
{
    throw new InvalidOperationException(
        "MyRateLimit:Window deve ser maior que zero.");
}

if (myOptions.QueueLimit < 0)
{
    throw new InvalidOperationException(
        "MyRateLimit:QueueLimit não pode ser negativo.");
}




// rate limite global
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpcontext => RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey:httpcontext.Connection.RemoteIpAddress?.ToString()?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = myOptions.AutoReplenishment,
                            PermitLimit = myOptions.PermitLimit,
                            QueueLimit = myOptions.QueueLimit,
                            Window = TimeSpan.FromSeconds(myOptions.Window)
                        }));
});









builder.Services.AddIdentity<Usuario, IdentityRole>().AddEntityFrameworkStores<ApiContext>().AddDefaultTokenProviders();







builder.Services.AddScoped(typeof(IRepository<>), typeof(Repositorio<>));
builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
builder.Services.AddScoped<IUsuariosRepository,UsuariosRepositorio>();
builder.Services.AddScoped<IEventosRepository,EventosRepositorio>();
builder.Services.AddScoped<IEventoServices,EventosServices>();
builder.Services.AddScoped<IUsuarioServices,UsuariosServices>();
builder.Services.AddScoped<IInscricaoRepository, InscricaoRepository>();
builder.Services.AddScoped<IInscricaoServices, InscricaoServices>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITurmasRepository, TurmasRepository>();
builder.Services.AddScoped<IInscricaoAppRepository, InscricaoAppRepository>();
builder.Services.AddScoped<IInscricaoAppServices, InscricaoAppServices>();
builder.Services.AddScoped<ITurmasServices, TurmasServices>();


builder.Services.AddAutoMapper(cfg => { },
    typeof(DomainDTOMappingProfile));







builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiLoggingFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        ReferenceHandler.IgnoreCycles;

    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/api-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        fileSizeLimitBytes: 10_000_000,
        rollOnFileSizeLimit: true,
        shared: false)
    .CreateLogger();

builder.Host.UseSerilog();


builder.Services.AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.SecretKey),
        "JWT:SecretKey não configurado.")
    .Validate(options =>
        options.SecretKey.Length >= 32,
        "JWT:SecretKey deve possuir pelo menos 32 caracteres.")
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.ValidIssuer),
        "JWT:ValidIssuer não configurado.")
    .Validate(options =>
        !string.IsNullOrWhiteSpace(options.ValidAudience),
        "JWT:ValidAudience não configurado.")
    .ValidateOnStart();




builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer();


builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtOptions>>(
        (jwtBearerOptions, jwtOptionsAccessor) =>
        {
            var jwtOptions = jwtOptionsAccessor.Value;

            jwtBearerOptions.SaveToken = true;

            jwtBearerOptions.RequireHttpsMetadata =
                !builder.Environment.IsDevelopment();

            jwtBearerOptions.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.ValidIssuer,

                    ValidateAudience = true,
                    ValidAudience = jwtOptions.ValidAudience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                jwtOptions.SecretKey)),

                    ClockSkew = TimeSpan.Zero,

                    NameClaimType = ClaimTypes.Name,

                    RoleClaimType = ClaimTypes.Role
                };
        });





builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin", "SuperAdmin"));


    options.AddPolicy("Super", policy =>
    {
        policy.RequireRole("SuperAdmin");

        policy.RequireAssertion(context =>
        {
            var userId = context.User.FindFirst("userId")?.Value;

            return userId == superAdminUserId;
        });
    });


    options.AddPolicy("User", policy =>
    policy.RequireRole("User", "Admin", "SuperAdmin"));
});











builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiSolution", Version = "v1" });

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "APISolution",
        Description = "API para plataforma de corrida para o orgão da ADRI",
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
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT",
    });

    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});



builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    // Substitua pelo IP real do seu proxy reverso.
    // Não use KnownNetworks.Clear() ou KnownProxies.Clear()
    // sem saber exatamente o que está fazendo.
    options.KnownProxies.Add(
        IPAddress.Parse("IP_DO_SEU_PROXY"));
});






var app = builder.Build();

app.ConfigureExceptionMiddlewareExtensions();

app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "APISolution v1");
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


public partial class Program { }