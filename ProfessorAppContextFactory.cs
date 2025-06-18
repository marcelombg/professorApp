using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProfessorApp.Api.Data;
using System.IO;

public class ProfessorAppContextFactory : IDesignTimeDbContextFactory<ProfessorAppContext>
{
    public ProfessorAppContext CreateDbContext(string[] args)
    {
        // Carrega o appsettings.json ou outro arquivo de config
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json") // ou appsettings.Development.json
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var optionsBuilder = new DbContextOptionsBuilder<ProfessorAppContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new ProfessorAppContext(optionsBuilder.Options);

        
    }
}
