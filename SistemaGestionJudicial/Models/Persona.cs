using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SistemaGestionJudicial.Models;
using System;

namespace SistemaGestionJudicial.Models
{
    /// <summary>
    /// Representa a cualquier persona del sistema (jueces, fiscales, policías, ciudadanos, etc.)
    /// </summary>
    public class Persona
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    public string? Genero { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string? CorreoElectronico { get; set; }

    public virtual ICollection<Denuncia> Denuncia { get; set; } = new List<Denuncia>();

    public virtual ICollection<Fiscale> Fiscales { get; set; } = new List<Fiscale>();

    public virtual Role? IdRolNavigation { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }
    public virtual ICollection<Juicio> Juicios { get; set; } = new List<Juicio>();

        [StringLength(20)]
        public string Telefono { get; set; }
    public virtual ICollection<JuiciosAcusado> JuiciosAcusados { get; set; } = new List<JuiciosAcusado>();

        [StringLength(200)]
        public string CorreoElectronico { get; set; }
    public virtual ICollection<PartesPoliciale> PartesPoliciales { get; set; } = new List<PartesPoliciale>();

        /// <summary>
        /// Propiedad de navegación para la tabla Rol.
        /// Esta propiedad no se valida ni se enlaza desde formularios.
        /// </summary>
        [ValidateNever] // ✅ ¡Esto es lo que evita el error!
        public virtual Rol Rol { get; set; }
    }
    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
