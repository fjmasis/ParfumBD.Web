using ParfumBD.Web.Models;
using ParfumBD.Web.Service;
using System.Text.Json;

namespace ParfumBD.Web.Services
{
    public class CarritoService : ICarritoService
    {
        private readonly IApiService _apiService;
        private readonly IPerfumeService _perfumeService;
        private readonly ILogger<CarritoService> _logger;
        private const string CarritoEndpoint = "api/carritos";
        private const string DetalleCarritoEndpoint = "api/detallecarrito";

        public CarritoService(
            IApiService apiService,
            IPerfumeService perfumeService,
            ILogger<CarritoService> logger)
        {
            _apiService = apiService;
            _perfumeService = perfumeService;
            _logger = logger;
        }

        public async Task<Carrito?> GetCarritoAsync(int idUsuario)
        {
            try
            {
                _logger.LogInformation($"Getting cart for user {idUsuario}");
                return await _apiService.GetAsync<Carrito>($"{CarritoEndpoint}/usuario/{idUsuario}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart for user {idUsuario}");
                return null;
            }
        }

        public async Task<Carrito?> AddToCarritoAsync(int idUsuario, int idPerfume, int cantidad)
        {
            try
            {
                _logger.LogInformation($"Adding product {idPerfume} to cart for user {idUsuario}");

                // First, check if the user already has an active cart
                var carrito = await GetCarritoAsync(idUsuario);

                if (carrito == null)
                {
                    _logger.LogInformation($"Creating new cart for user {idUsuario}");
                    // Create a new cart
                    var newCarritoDto = new
                    {
                        idUsuario,
                        estado = "Activo"
                    };

                    carrito = await _apiService.PostAsync<Carrito, object>(CarritoEndpoint, newCarritoDto);

                    if (carrito == null)
                    {
                        _logger.LogError($"Failed to create cart for user {idUsuario}");
                        return null;
                    }
                }

                // Get the perfume to get its price
                var perfume = await _perfumeService.GetPerfumeByIdAsync(idPerfume);
                if (perfume == null)
                {
                    _logger.LogError($"Perfume {idPerfume} not found");
                    return null;
                }

                // Check if the item is already in the cart
                var existingItem = carrito.DetallesCarrito?.FirstOrDefault(d => d.IdPerfume == idPerfume);

                if (existingItem != null)
                {
                    _logger.LogInformation($"Updating existing cart item {existingItem.IdDetalle}");
                    // Update the quantity
                    await UpdateCarritoItemAsync(existingItem.IdDetalle, existingItem.Cantidad + cantidad);
                }
                else
                {
                    _logger.LogInformation($"Adding new item to cart {carrito.IdCarrito}");
                    // Add a new item to the cart
                    var detalleCarritoDto = new
                    {
                        idCarrito = carrito.IdCarrito,
                        idPerfume,
                        cantidad,
                        precioUnitario = perfume.Precio
                    };

                    var result = await _apiService.PostAsync<DetalleCarrito, object>(DetalleCarritoEndpoint, detalleCarritoDto);
                    if (result == null)
                    {
                        _logger.LogError($"Failed to add item to cart {carrito.IdCarrito}");
                    }
                }

                // Get the updated cart
                return await GetCarritoAsync(idUsuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding product {idPerfume} to cart for user {idUsuario}");
                return null;
            }
        }

        public async Task<bool> UpdateCarritoItemAsync(int idDetalle, int cantidad)
        {
            try
            {
                _logger.LogInformation($"Updating cart item {idDetalle} to quantity {cantidad}");
                var updateDto = new
                {
                    cantidad,
                    precioUnitario = 0.0m // This will be ignored as we're not updating the price
                };

                var result = await _apiService.PutAsync<DetalleCarrito, object>(DetalleCarritoEndpoint, idDetalle, updateDto);
                return result != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating cart item {idDetalle}");
                return false;
            }
        }

        public async Task<bool> RemoveFromCarritoAsync(int idDetalle)
        {
            try
            {
                _logger.LogInformation($"Removing cart item {idDetalle}");
                return await _apiService.DeleteAsync(DetalleCarritoEndpoint, idDetalle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cart item {idDetalle}");
                return false;
            }
        }
    }
}