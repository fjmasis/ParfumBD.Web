namespace ParfumBD.Web.Models
{
    public class Carrito
    {
        public int IdCarrito { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Estado { get; set; }
        public List<DetalleCarrito>? DetallesCarrito { get; set; } = new List<DetalleCarrito>();
        public decimal Total => DetallesCarrito?.Sum(d => d.Subtotal) ?? 0;
    }

    public class DetalleCarrito
    {
        public int IdDetalle { get; set; }
        public int IdCarrito { get; set; }
        public int IdPerfume { get; set; }
        public string? NombrePerfume { get; set; }
        public string? ImagenPerfume { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    public class AgregarAlCarritoViewModel
    {
        public int IdPerfume { get; set; }
        public int Cantidad { get; set; } = 1;
    }
}
