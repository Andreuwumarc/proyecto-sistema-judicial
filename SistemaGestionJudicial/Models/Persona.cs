using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionJudicial.Models
{
    [Table("personas")]
    public class Persona
    {
        public long Id_Persona { get; set; }
        public string? Cedula { get; set; }
        public string? Nombres { get; set; }
        public string? Apellidos { get; set; }

        [ForeignKey("Rol")]
        public long Id_Rol { get; set; }
        public Rol? Rol { get; set; }

    }
}
