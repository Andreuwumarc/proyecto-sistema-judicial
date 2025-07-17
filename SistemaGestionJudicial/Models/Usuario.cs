using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionJudicial.Models
{
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public long IdUsuario { get; set; }

        [Required]
        [Column("id_persona")]
        public long IdPersona { get; set; }

        [Required]
        [StringLength(50)]
        [Column("nombre_usuario")]
        public string NombreUsuario { get; set; }

        [Required]
        [StringLength(255)]
        [Column("contrasena")]
        public string Contrasena { get; set; }

        [StringLength(50)]
        [Column("rol_usuario")]
        public string RolUsuario { get; set; }

        // Navegación: Persona relacionada (FK)
        [ForeignKey("IdPersona")]
        public virtual Persona Persona { get; set; }
    }
}
