using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ProfessorApp.Api.Models
{
    public class Aluno
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Nome { get; set; } = string.Empty;
        
        public DateTime? DataNascimento { get; set; }
        
        public bool Ativo { get; set; } = true;

        // Propriedade de navegação
        [JsonIgnore]
        public virtual ICollection<Aprendizado> Aprendizados { get; set; } = new List<Aprendizado>();
    }
}

