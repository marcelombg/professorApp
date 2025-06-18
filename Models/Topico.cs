using System.ComponentModel.DataAnnotations;

namespace ProfessorApp.Api.Models
{
    public class Topico
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Nome { get; set; } = string.Empty;
        
        // Propriedade de navegação para o relacionamento N:N
        public virtual ICollection<Aprendizado> Aprendizados { get; set; } = new List<Aprendizado>();
    }
}

