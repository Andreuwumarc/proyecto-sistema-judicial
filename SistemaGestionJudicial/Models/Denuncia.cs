using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaGestionJudicial.Models
{
    [Table("denuncias")]
    public class Denuncia
    {

        public long Id_Denuncia { get; set; }  // Clave primaria

        // Otras propiedades que tenga tu modelo
        public string? Descripcion { get; set; }

        // Puedes agregar más campos si los tienes
    }
}
