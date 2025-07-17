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
        public long? IdPersona { get; set; }

        [Required]
        [Column("usuario")]
        [StringLength(50)]
        public string Usuario1 { get; set; } = null!; // ✅ Debe coincidir con DB y scaffolding

        [Required]
        [Column("contraseña")]
        [StringLength(255)]
        public string Contraseña { get; set; } = null!; // ✅ coincide con scaffold

        [Column("token")]
        [StringLength(50)]
        public string? Token { get; set; }

        // Navegación
        [ForeignKey("IdPersona")]
        public virtual Persona? IdPersonaNavigation { get; set; }
    }
}
