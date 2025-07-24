namespace SistemaGestionJudicial.Models
{
    public class RegisterViewModel
    {
        public string Cedula { get; set; } = null!;
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; } = null!;
        public string? Direccion { get; set; }
        public string Telefono { get; set; } = null!;
        public string CorreoElectronico { get; set; } = null!;
        public long IdRol { get; set; }
        public string Contrasena { get; set; } = null!;
    }
}
