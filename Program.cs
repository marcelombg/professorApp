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

// Converte URI do Railway para Npgsql
string connectionString;
if (databaseUrl.StartsWith("postgres://"))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    connectionString =
        $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};" +
        $"Username={userInfo[0]};Password={userInfo[1]};Ssl Mode=Require;Trust Server Certificate=true;";
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

// -------------------------
// Configura CORS
// -------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// -------------------------
// Adiciona controllers e swagger
// -------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -------------------------
// Aplica migrações automaticamente
// -------------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProfessorAppContext>();
    db.Database.Migrate();
}

// -------------------------
// Configura middleware
// -------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors();
app.MapControllers();

// -------------------------
// Configura porta dinâmica do Railway
// -------------------------
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();