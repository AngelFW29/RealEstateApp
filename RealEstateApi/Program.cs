using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using RealEstateApi.Middlewares;
using RealEstateApp.Core.Application;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Application.Services;
using RealEstateApp.Infrastructure.Identity;
using RealEstateApp.Infrastructure.Persistence;
using RealEstateApp.Infrastructure.Shared;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddPersistenceLayer(builder.Configuration);
builder.Services.AddIdentityForWebApi(builder.Configuration);
builder.Services.AddSharedLayer(builder.Configuration);
builder.Services.AddApplicationLayer();
// Añadir esta configuración de respuesta de validación justo debajo de builder.Services.AddControllers();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(new
            {
                Status = 400,
                Message = "El estado enviado no es válido."
            });
        };
    });

// Registrar el servicio en DI
builder.Services.AddScoped<IAgentApiService, AgentApiService>();

// Configurar Swagger con Bearer Token
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "RealEstateApp API", Version = "v1" });
    o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega SOLO el token aquí"
    });
    o.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Ejecutar Seeds
await app.Services.RunPersistenceSeedAsync();
await app.Services.RunIdentitySeedAsync();

app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();