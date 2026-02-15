using System.Net.Http.Headers;
using System.Text;
using ChatDashboard.Api.Models;

namespace ChatDashboard.Api.Services
{
    public class UserService
    {
        private readonly HttpClient _httpClient;
        private readonly string _dbName = "chat_app_db";

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            var byteArray = Encoding.ASCII.GetBytes("mehar:Rbh@123");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var query = new
            {
                selector = new { type = "user" }
            };

            var response = await _httpClient.PostAsJsonAsync($"http://localhost:5984/{_dbName}/_find", query);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CouchDbFindResponse<User>>();

            return result.Docs;
        }
    }
}