using Microsoft.EntityFrameworkCore;
using ProfessorApp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Lê a connection string do Railway
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
string connectionString;

if (!string.IsNullOrEmpty(databaseUrl) && databaseUrl.StartsWith("postgres://"))
{
    // Converte URI do Railway para formato Npgsql
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString =
        $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={userInfo[0]};Password={userInfo[1]};Ssl Mode=Require;Trust Server Certificate=true;";
}
else
{
    // Fallback local
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// Configura o DbContext
builder.Services.AddDbContext<ProfessorAppContext>(options =>
    options.UseNpgsql(connectionString));

// Adiciona controllers e endpoints
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// Porta 8080 exigida pelo Railway
app.Urls.Clear();
app.Urls.Add("http://+:8080");

app.Run();
