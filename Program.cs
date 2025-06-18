using Microsoft.EntityFrameworkCore;
using ProfessorApp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte para variáveis de ambiente sobrescreverem configurações
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<ProfessorAppContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection") ??
        throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
    ));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usar CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ProfessorAppContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao criar o banco de dados: {ex.Message}");
        throw;
    }
}

await app.RunAsync("http://0.0.0.0:5000");

