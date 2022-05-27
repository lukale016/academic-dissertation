using MedicalRemoteCommunicationSupport.Services;
using MedicalRemoteCommunicationSupport.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Configuration
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));
// CORS
builder.Services.AddCors();
// SignalR
builder.Services.AddSignalRCore();
// Singletons
builder.Services.AddSingleton<UnitOfWork>();
builder.Services.AddSingleton<IFileManager, FileManager>();
// Transients
builder.Services.AddTransient<IRedisHelperService, RedisHelperService>();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
