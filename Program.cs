using Microsoft.EntityFrameworkCore;
using ProfessorApp.Api.Data; // ajuste para o namespace do seu DbContext

var builder = WebApplication.CreateBuilder(args);

// Obtém a URL do banco do ambiente
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(databaseUrl))
{
    throw new Exception("DATABASE_URL não encontrada no ambiente!");
}

// Converte URI do Railway para string de conexão Npgsql
var tempUrl = databaseUrl.Replace("postgres://", "http://").Replace("postgresql://", "http://");
var uri = new Uri(tempUrl);
var userInfo = uri.UserInfo.Split(':');

var connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};" +
                       $"Username={userInfo[0]};Password={userInfo[1]};Ssl Mode=Require;Trust Server Certificate=true;";

// Configura o DbContext
builder.Services.AddDbContext<ProfessorAppContext>(options =>
    options.UseNpgsql(connectionString));

// Configura serviços do ASP.NET
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica migrations automaticamente no startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProfessorAppContext>();
    db.Database.Migrate();
}

// Configura pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
