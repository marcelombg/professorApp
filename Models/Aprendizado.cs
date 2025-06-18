using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProfessorApp.Api.Models
{
    public class Aprendizado
    {
        public int Id { get; set; }
        
        [Required]
        public int AlunoId { get; set; }
        
        [Required]
        public DateTime DataRegistro { get; set; } = DateTime.Now;
        
        [Required]
        public string Descricao { get; set; } = string.Empty;
        
        // Propriedades de navegação
        public virtual Aluno Aluno { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Topico> Topicos { get; set; } = new List<Topico>();
    }
}

