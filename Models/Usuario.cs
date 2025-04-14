namespace ParfumBD.Web.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string? Nombre { get; set; }
        public string? Correo { get; set; }
        public string? TipoUsuario { get; set; }
        public DateTime FechaRegistro { get; set; }
    }

    public class UsuarioLogin
    {
        public string Correo { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public bool RecordarMe { get; set; }
    }

    public class UsuarioRegistro
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public string ConfirmarContraseña { get; set; } = string.Empty;
    }
}
