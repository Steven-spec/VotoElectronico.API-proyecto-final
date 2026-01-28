using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;


namespace VotoElectronico.MVC.Services
{
    public interface IApiConsumer
    {
        Task<T?> GetAsync<T>(string endpoint, string? token = null);
        Task<T?> PostAsync<T>(string endpoint, object data, string? token = null);
        Task<T?> PutAsync<T>(string endpoint, object data, string? token = null);
        Task<bool> DeleteAsync(string endpoint, string? token = null);
    }

    public class ApiConsumer : IApiConsumer
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ApiConsumer(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5050/");
        }

        public async Task<T?> GetAsync<T>(string endpoint, string? token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.GetAsync(endpoint);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }

                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetAsync: {ex.Message}");
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string endpoint, object data, string? token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseJson);
                }

                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PostAsync: {ex.Message}");
                return default;
            }
        }

        public async Task<T?> PutAsync<T>(string endpoint, object data, string? token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseJson);
                }

                return default;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PutAsync: {ex.Message}");
                return default;
            }
        }

        public async Task<bool> DeleteAsync(string endpoint, string? token = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.DeleteAsync(endpoint);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DeleteAsync: {ex.Message}");
                return false;
            }
        }
    }
}