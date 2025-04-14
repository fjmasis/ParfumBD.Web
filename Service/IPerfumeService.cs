using ParfumBD.Web.Models;

namespace ParfumBD.Web.Services
{
    public interface IPerfumeService
    {
        Task<IEnumerable<Perfume>?> GetAllPerfumesAsync();
        Task<IEnumerable<Perfume>?> GetActivePerfumesAsync();
        Task<Perfume?> GetPerfumeByIdAsync(int id);
        Task<IEnumerable<Perfume>?> GetPerfumesByMarcaAsync(string marca);
    }
}