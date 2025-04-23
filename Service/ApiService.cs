using ParfumBD.Web.Service;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ParfumBD.Web.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ApiService> _logger;

        public ApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ApiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            // Set base address from configuration
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"]);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private void SetAuthToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("AuthToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            try
            {
                SetAuthToken();
                _logger.LogInformation($"GET request to {endpoint}");
                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"GET response from {endpoint}: {content.Substring(0, Math.Min(content.Length, 100))}...");
                    return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"GET request to {endpoint} failed with status {response.StatusCode}: {errorContent}");
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in GET request to {endpoint}");
                throw;
            }
        }

        public async Task<T?> GetByIdAsync<T>(string endpoint, int id)
        {
            try
            {
                SetAuthToken();
                _logger.LogInformation($"GET request to {endpoint}/{id}");
                var response = await _httpClient.GetAsync($"{endpoint}/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"GET request to {endpoint}/{id} failed with status {response.StatusCode}: {errorContent}");
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in GET request to {endpoint}/{id}");
                throw;
            }
        }

        public async Task<T?> PostAsync<T, U>(string endpoint, U data)
        {
            try
            {
                SetAuthToken();
                var json = JsonSerializer.Serialize(data);
                _logger.LogInformation($"POST request to {endpoint} with data: {json}");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"POST response from {endpoint}: {responseContent.Substring(0, Math.Min(responseContent.Length, 100))}...");
                    return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"POST request to {endpoint} failed with status {response.StatusCode}: {errorContent}");
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in POST request to {endpoint}");
                throw;
            }
        }

        public async Task<T?> PutAsync<T, U>(string endpoint, int id, U data)
        {
            try
            {
                SetAuthToken();
                var json = JsonSerializer.Serialize(data);
                _logger.LogInformation($"PUT request to {endpoint}/{id} with data: {json}");
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{endpoint}/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(responseContent))
                    {
                        return default;
                    }
                    return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"PUT request to {endpoint}/{id} failed with status {response.StatusCode}: {errorContent}");
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in PUT request to {endpoint}/{id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint, int id)
        {
            try
            {
                SetAuthToken();
                _logger.LogInformation($"DELETE request to {endpoint}/{id}");
                var response = await _httpClient.DeleteAsync($"{endpoint}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"DELETE request to {endpoint}/{id} failed with status {response.StatusCode}: {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in DELETE request to {endpoint}/{id}");
                throw;
            }
        }
    }
}