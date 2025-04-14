using ParfumBD.Web.Models;
using ParfumBD.Web.Service;

namespace ParfumBD.Web.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IApiService _apiService;
        private const string Endpoint = "api/usuarios";

        public UsuarioService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<Usuario?> LoginAsync(UsuarioLogin login)
        {
            return await _apiService.PostAsync<Usuario, UsuarioLogin>($"{Endpoint}/login", login);
        }

        public async Task<Usuario?> RegisterAsync(UsuarioRegistro registro)
        {
            var usuarioDto = new
            {
                nombre = registro.Nombre,
                correo = registro.Correo,
                contraseña = registro.Contraseña,
                tipoUsuario = "Cliente" // Default to Cliente for new registrations
            };

            return await _apiService.PostAsync<Usuario, object>(Endpoint, usuarioDto);
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            return await _apiService.GetByIdAsync<Usuario>(Endpoint, id);
        }

        public async Task<Usuario?> UpdateUsuarioAsync(int id, Usuario usuario)
        {
            var usuarioDto = new
            {
                nombre = usuario.Nombre,
                correo = usuario.Correo,
                tipoUsuario = usuario.TipoUsuario
            };

            return await _apiService.PutAsync<Usuario, object>(Endpoint, id, usuarioDto);
        }
    }
}