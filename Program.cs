using Microsoft.EntityFrameworkCore;
using ProfessorApp.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// Configura DATABASE_URL
// -------------------------
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(databaseUrl))
{
    throw new Exception("DATABASE_URL não encontrada no ambiente!");
}

string connectionString;

// Se for URL estilo postgres://
if (databaseUrl.StartsWith("postgres://") || databaseUrl.StartsWith("postgresql://"))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString =
        $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={userInfo[0]};Password={userInfo[1]};Ssl Mode=Require;Trust Server Certificate=true;";
}
// Se for só o host interno (postgres.railway.internal)
else if (databaseUrl.Contains("railway.internal"))
{
    var user = Environment.GetEnvironmentVariable("PGUSER") ?? "postgres";
    var password = Environment.GetEnvironmentVariable("PGPASSWORD") ?? "";
    var dbName = Environment.GetEnvironmentVariable("PGDATABASE") ?? "railway";
    var port = Environment.GetEnvironmentVariable("PGPORT") ?? "5432";

    connectionString =
        $"Host={databaseUrl};Port={port};Database={dbName};" +
        $"Username={user};Password={password};Ssl Mode=Require;Trust Server Certificate=true;";
}
else
{
    connectionString = databaseUrl;
}

// -------------------------
// Configura DbContext
// -------------------------
builder.Services.AddDbContext<ProfessorAppContext>(options =>
    options.UseNpgsql(connectionString));

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica migrações
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProfessorAppContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors();
app.MapControllers();

var portEnv = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{portEnv}");

app.Run();
