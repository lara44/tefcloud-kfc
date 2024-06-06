using tefcloud_token.Services;
using tefcloud_token.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configurar Kestrel para escuchar en una URL específica
//builder.WebHost.UseUrls("http://0.0.0.0:8080");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<Token, TokenService>();
builder.Services.AddSingleton<Redis, RedisService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
