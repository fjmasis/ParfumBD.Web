using ParfumBD.Web.Models;
using ParfumBD.Web.Service;

namespace ParfumBD.Web.Services
{
    public class CarritoService : ICarritoService
    {
        private readonly IApiService _apiService;
        private readonly IPerfumeService _perfumeService;
        private const string CarritoEndpoint = "api/carritos";
        private const string DetalleCarritoEndpoint = "api/detallecarrito";

        public CarritoService(IApiService apiService, IPerfumeService perfumeService)
        {
            _apiService = apiService;
            _perfumeService = perfumeService;
        }

        public async Task<Carrito?> GetCarritoAsync(int idUsuario)
        {
            // This would need to be implemented in the API to get a cart by user ID
            return await _apiService.GetAsync<Carrito>($"{CarritoEndpoint}/usuario/{idUsuario}");
        }

        public async Task<Carrito?> AddToCarritoAsync(int idUsuario, int idPerfume, int cantidad)
        {
            // First, check if the user already has an active cart
            var carrito = await GetCarritoAsync(idUsuario);

            if (carrito == null)
            {
                // Create a new cart
                var newCarrito = new
                {
                    idUsuario,
                    estado = "Activo"
                };

                carrito = await _apiService.PostAsync<Carrito, object>(CarritoEndpoint, newCarrito);

                if (carrito == null)
                {
                    return null;
                }
            }

            // Get the perfume to get its price
            var perfume = await _perfumeService.GetPerfumeByIdAsync(idPerfume);
            if (perfume == null)
            {
                return null;
            }

            // Check if the item is already in the cart
            var existingItem = carrito.DetallesCarrito?.FirstOrDefault(d => d.IdPerfume == idPerfume);

            if (existingItem != null)
            {
                // Update the quantity
                await UpdateCarritoItemAsync(existingItem.IdDetalle, existingItem.Cantidad + cantidad);
            }
            else
            {
                // Add a new item to the cart
                var detalleCarrito = new
                {
                    idCarrito = carrito.IdCarrito,
                    idPerfume,
                    cantidad,
                    precioUnitario = perfume.Precio
                };

                await _apiService.PostAsync<DetalleCarrito, object>(DetalleCarritoEndpoint, detalleCarrito);
            }

            // Get the updated cart
            return await GetCarritoAsync(idUsuario);
        }

        public async Task<bool> UpdateCarritoItemAsync(int idDetalle, int cantidad)
        {
            var updateDto = new
            {
                cantidad,
                precioUnitario = 0.0m // This will be ignored as we're not updating the price
            };

            var result = await _apiService.PutAsync<DetalleCarrito, object>(DetalleCarritoEndpoint, idDetalle, updateDto);
            return result != null;
        }

        public async Task<bool> RemoveFromCarritoAsync(int idDetalle)
        {
            return await _apiService.DeleteAsync(DetalleCarritoEndpoint, idDetalle);
        }
    }
}