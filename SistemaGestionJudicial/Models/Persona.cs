using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SistemaGestionJudicial.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [StringLength(200)]
        public string Direccion { get; set; }

        [StringLength(20)]
        public string Telefono { get; set; }

        [StringLength(200)]
        public string CorreoElectronico { get; set; }

        /// <summary>
        /// Propiedad de navegación para la tabla Rol.
        /// Esta propiedad no se valida ni se enlaza desde formularios.
        /// </summary>
        [ValidateNever] // ✅ ¡Esto es lo que evita el error!
        public virtual Rol Rol { get; set; }
    }
}
