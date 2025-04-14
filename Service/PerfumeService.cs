using ParfumBD.Web.Models;
using ParfumBD.Web.Service;

namespace ParfumBD.Web.Services
{
    public class PerfumeService : IPerfumeService
    {
        private readonly IApiService _apiService;
        private const string Endpoint = "api/perfumes";

        public PerfumeService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<Perfume>?> GetAllPerfumesAsync()
        {
            return await _apiService.GetAsync<IEnumerable<Perfume>>(Endpoint);
        }

        public async Task<IEnumerable<Perfume>?> GetActivePerfumesAsync()
        {
            return await _apiService.GetAsync<IEnumerable<Perfume>>($"{Endpoint}/active");
        }

        public async Task<Perfume?> GetPerfumeByIdAsync(int id)
        {
            return await _apiService.GetByIdAsync<Perfume>(Endpoint, id);
        }

        public async Task<IEnumerable<Perfume>?> GetPerfumesByMarcaAsync(string marca)
        {
            return await _apiService.GetAsync<IEnumerable<Perfume>>($"{Endpoint}/marca/{marca}");
        }
    }
}
