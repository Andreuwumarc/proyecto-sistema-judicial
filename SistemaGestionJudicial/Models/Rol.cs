using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionJudicial.Models
{
    /// <summary>
    /// Representa el rol o función que una persona puede desempeñar (Ej: Juez, Fiscal, Policía, etc.)
    /// </summary>
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long IdRol { get; set; }

        [Required]
        [StringLength(30)]
        public string Nombre { get; set; }

        // Navigation property para la relación uno a muchos con Personas
        public virtual ICollection<Persona>? Personas { get; set; }
    }
}
