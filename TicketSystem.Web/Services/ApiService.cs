using System.Net.Http.Json;
using TicketSystem.Web.Models;

namespace TicketSystem.Web.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public ApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _httpClient.BaseAddress = new Uri(_config["ApiSettings:BaseUrl"]);
        }

        public async Task<List<Event>> GetEventsAsync(string? search = null, int? categoryId = null, string? city = null)
        {
            var query = $"/api/Events?search={search}&categoryId={categoryId}&city={city}";
            return await _httpClient.GetFromJsonAsync<List<Event>>(query);
        }

        public async Task<Event> GetEventAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Event>($"/api/Events/{id}");
        }

        public async Task BuyTicketAsync(int eventId, int row, int seat)
        {
            var request = new { Row = row, Seat = seat };
            await _httpClient.PostAsJsonAsync($"/api/Events/{eventId}/buy", request);
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", new { Email = email, Password = password });
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result.Token;
        }

        public async Task CreateEventAsync(Event evt, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("/api/Events", evt);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteEventAsync(int id, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            await _httpClient.DeleteAsync($"/api/Events/{id}");
        }

        public class LoginResponse
        {
            public string Token { get; set; }
        }
    }
}