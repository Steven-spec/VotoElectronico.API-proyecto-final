using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using VotoElectronico.APII.Data;
using VotoElectronico.APII.Services;

namespace VotoElectronico.APII
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configurar DbContext
            builder.Services.AddDbContext<VotoElectronicoContext>(options =>
                options.UseMySql(
                    builder.Configuration.GetConnectionString("VotoElectronicoContext.mariadb")
                        ?? throw new InvalidOperationException("Connection string not found."),
                    Microsoft.EntityFrameworkCore.ServerVersion.Parse("11.0.2-MariaDB")
                )
            );

            // Registrar servicios
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IEncriptacionService, EncriptacionService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ISmsService, SmsService>();

            // Configurar JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            builder.Services.AddAuthorization();

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Configurar Swagger con JWT
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "VotoElectronico API",
                    Version = "v1",
                    Description = "API para Sistema de Voto Electrónico Seguro"
                });

                // Configurar JWT en Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Configure JSON options para evitar referencias circulares
            builder.Services
                .AddControllers()
                .AddNewtonsoftJson(
                    options =>
                    options.SerializerSettings.ReferenceLoopHandling
                    = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            // Configurar CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VotoElectronico API V1");
            });

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthentication(); // IMPORTANTE: Debe ir antes de Authorization
            app.UseAuthorization();

            app.MapControllers();

            // Seed data de prueba - COMENTADO TEMPORALMENTE
            /*
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<VotoElectronicoContext>();
                    var encriptacion = services.GetRequiredService<IEncriptacionService>();
                    DataSeeder.SeedData(context, encriptacion).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al inicializar datos: {ex.Message}");
                }
            }
            */

            app.Run();
        }
    }
}