using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Repositories;
using BoxExpress.Application.Services;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Application.Interfaces;
using BoxExpress.Application.Mappings;
using BoxExpress.Application.Extensions;
using AutoMapper;
using BoxExpress.Infrastructure.Extensions;
using BoxExpress.Application.Configurations;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using BoxExpress.Api.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// DbContext
builder.Services.AddDbContext<BoxExpressDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Controllers
builder.Services.AddControllers();

// JWT Auth
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Application & Infrastructure
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration.GetConnectionString("DefaultConnection")!);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "BoxExpress.Api", Version = "v1" });
});

var app = builder.Build();

// Swagger (solo para dev/prod)
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BoxExpress API v1");
    });
}

// Middleware orden correcto
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Redirigir raíz a Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.Run();


// 💡 Principios básicos:
// Domain: solo conoce entidades y lógica de negocio pura.

// Application: usa DTOs para comunicar datos entre capas (como entre API y Services).

// Infrastructure: implementa los contratos de Domain, sin introducir lógica de negocio.

// API: es la capa externa que consume Application.

// se usa para no romper la inversión de dependencias.

//Aislamiento de capas

// “Duplication is better than the wrong abstraction.”
// — Sandi Metz

//dotnet run --project BoxExpress.Api/BoxExpress.Api.csproj