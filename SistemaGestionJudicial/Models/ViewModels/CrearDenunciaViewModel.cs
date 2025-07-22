namespace SistemaGestionJudicial.Models.ViewModels
{
    public class CrearDenunciaViewModel
    {
        public Persona DatosPersona { get; set; }
        public List<Delito> TiposDelito { get; set; }
        public Denuncia NuevaDenuncia { get; set; }

    }
}
