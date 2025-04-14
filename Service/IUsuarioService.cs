using ParfumBD.Web.Models;

namespace ParfumBD.Web.Services
{
    public interface IUsuarioService
    {
        Task<Usuario?> LoginAsync(UsuarioLogin login);
        Task<Usuario?> RegisterAsync(UsuarioRegistro registro);
        Task<Usuario?> GetUsuarioByIdAsync(int id);
        Task<Usuario?> UpdateUsuarioAsync(int id, Usuario usuario);
    }
}