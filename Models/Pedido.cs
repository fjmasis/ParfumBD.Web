namespace ParfumBD.Web.Models
{
    public class Pedido
    {
        public int IdPedido { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal Total { get; set; }
        public string? Estado { get; set; }
        public List<DetallePedido>? DetallesPedido { get; set; }
    }

    public class DetallePedido
    {
        public int IdDetalle { get; set; }
        public int IdPedido { get; set; }
        public int IdPerfume { get; set; }
        public string? NombrePerfume { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal => Cantidad * PrecioUnitario;
    }

    public class CheckoutViewModel
    {
        public string Provincia { get; set; } = string.Empty;
        public string Canton { get; set; } = string.Empty;
        public string DireccionExacta { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
    }
}
