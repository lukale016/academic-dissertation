using MedicalRemoteCommunicationSupport.Extensions;
using MedicalRemoteCommunicationSupport.Handlers;
using MedicalRemoteCommunicationSupport.Services;
using MedicalRemoteCommunicationSupport.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer()
                .AddSwaggerGen();
// Configuration
var dbSettings = builder.Configuration.GetSection("DbSettings");
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<DbSettings>(dbSettings);
builder.Services.Configure<JwtSettings>(jwtSettings);
// CORS
builder.Services.AddCors();
// SignalR
builder.Services.AddSignalR();
builder.Services.AddSignalRCore();
// Singletons
builder.Services.AddSingletons(dbSettings);
// Scoped
builder.Services.AddScoped<IHandler<Message>, MessageHandler>();
// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audiance"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SigningKey"]))
    };
});

if (!Directory.Exists(DirectoryPaths.UserData))
{
    Directory.CreateDirectory(DirectoryPaths.UserData);
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.AllowAnyHeader()
           .AllowAnyMethod()
           .SetIsOriginAllowed((host) => true)
           .AllowCredentials();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHubs();

app.Run();
