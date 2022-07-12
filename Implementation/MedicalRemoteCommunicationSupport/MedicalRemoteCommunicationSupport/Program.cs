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
builder.Services.AddSignalRCore();
// Singletons
builder.Services.AddSingleton<UnitOfWork>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IFileManager, FileManager>();
builder.Services.AddSingleton<ConnectionMultiplexer>(ConnectionMultiplexer.Connect(dbSettings["RedisConnectionUrl"]));
builder.Services.AddSingleton<MongoClient>(new MongoClient(dbSettings["MongoConnectionUrl"]));
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();
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
           .AllowAnyOrigin();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
