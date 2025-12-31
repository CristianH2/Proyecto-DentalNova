using DentalNova.Business.Helpers;
using DentalNova.Business.Rules;
using Microsoft.OpenApi.Models;
using System.Reflection;
using DentalNova.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddBusinessLogicServices(builder.Configuration);

// Registra ITokenService y la autenticación JWT
builder.Services.AddSecurityServices(builder.Configuration);

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Configura Swagger para usar autenticación JWT
builder.Services.AddSwaggerGen(options =>
{
    // Define el título de la API
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "DentalNova API", Version = "v1" });

    // documentación
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);

    // Configura el esquema de seguridad para JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", // El nombre del header HTTP
        Type = SecuritySchemeType.Http, // Tipo de esquema
        Scheme = "Bearer", // El esquema ("Bearer")
        BearerFormat = "JWT", // Formato del token
        In = ParameterLocation.Header, // Dónde se envía (en el header)
        Description = "Introduce tu token JWT (Ej: 'eyJhbGciOiJI...')"
    });

    // Aplica el esquema de seguridad globalmente a todas las operaciones
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer" // Debe coincidir con el Id de AddSecurityDefinition
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
    app.UseSwagger();
    app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
