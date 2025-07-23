using SistemaGestionJudicial.Models;

namespace SistemaGestionJudicial.Models
{
    public class CrearDenunciaViewModel
    {
        public Persona DatosPersona { get; set; }
        public List<Delito> TiposDelito { get; set; }
        public Denuncia NuevaDenuncia { get; set; }

    }
}