namespace ParfumBD.Web.Service
{
    public interface IApiService
    {
        Task<T?> GetAsync<T>(string endpoint);
        Task<T?> GetByIdAsync<T>(string endpoint, int id);
        Task<T?> PostAsync<T, U>(string endpoint, U data);
        Task<T?> PutAsync<T, U>(string endpoint, int id, U data);
        Task<bool> DeleteAsync(string endpoint, int id);
    }
}
