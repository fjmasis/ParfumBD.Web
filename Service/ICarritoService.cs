using ParfumBD.Web.Models;

namespace ParfumBD.Web.Services
{
    public interface ICarritoService
    {
        Task<Carrito?> GetCarritoAsync(int idUsuario);
        Task<Carrito?> AddToCarritoAsync(int idUsuario, int idPerfume, int cantidad);
        Task<bool> UpdateCarritoItemAsync(int idDetalle, int cantidad);
        Task<bool> RemoveFromCarritoAsync(int idDetalle);
    }
}
