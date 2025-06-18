using Microsoft.EntityFrameworkCore;
using ProfessorApp.Api.Models;

namespace ProfessorApp.Api.Data
{
    public class ProfessorAppContext : DbContext
    {
        public ProfessorAppContext(DbContextOptions<ProfessorAppContext> options) : base(options)
        {
        }
        
        public DbSet<Aluno> Alunos { get; set; }
        public DbSet<Topico> Topicos { get; set; }
        public DbSet<Aprendizado> Aprendizados { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuração da entidade Aluno
            modelBuilder.Entity<Aluno>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Ativo).HasDefaultValue(true);
            });
            
            // Configuração da entidade Topico
            modelBuilder.Entity<Topico>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.Nome).IsUnique();
            });
            
            // Configuração da entidade Aprendizado
            modelBuilder.Entity<Aprendizado>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Descricao).IsRequired();
                entity.Property(e => e.DataRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Relacionamento com Aluno (1:N)
                entity.HasOne(e => e.Aluno)
                      .WithMany(a => a.Aprendizados)
                      .HasForeignKey(e => e.AlunoId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Relacionamento N:N com Topicos
                entity.HasMany(e => e.Topicos)
                      .WithMany(t => t.Aprendizados)
                      .UsingEntity(j => j.ToTable("AprendizadoTopicos"));
            });
        }
    }
}

