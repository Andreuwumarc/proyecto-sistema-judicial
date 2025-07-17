using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionJudicial.Models
{
    public partial class Persona
    {
        [Key]
        public long IdPersona { get; set; }

        [Required]
        [StringLength(10)]
        public string Cedula { get; set; }

        [StringLength(100)]
        public string Nombres { get; set; }

        [StringLength(100)]
        public string Apellidos { get; set; }

        public DateTime? FechaNacimiento { get; set; }

        [ForeignKey("Rol")]
        public long? IdRol { get; set; }

        [StringLength(1)]
        public string Genero { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(200)]
        public string CorreoElectronico { get; set; }

        // Propiedades navegables
        [ValidateNever]
        public virtual Rol Rol { get; set; }

        [ValidateNever]
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

        [ValidateNever]
        public virtual ICollection<Juicio> Juicios { get; set; } = new List<Juicio>();

        [ValidateNever]
        public virtual ICollection<JuiciosAcusado> JuiciosAcusados { get; set; } = new List<JuiciosAcusado>();

        [ValidateNever]
        public virtual ICollection<Fiscale> Fiscales { get; set; } = new List<Fiscale>();

        [ValidateNever]
        public virtual ICollection<Denuncia> Denuncia { get; set; } = new List<Denuncia>();

        [ValidateNever]
        public virtual ICollection<PartesPoliciale> PartesPoliciales { get; set; } = new List<PartesPoliciale>();
    }
}
