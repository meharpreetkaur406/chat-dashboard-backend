using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using ChatDashboard.Api.Models;
using System.Text.Json;

namespace ChatDashboard.Api.Services 
{
    public class UserRegisterService
    {
        private readonly HttpClient _httpClient;
        private readonly string _dbName = "chat_app_db";

        public UserRegisterService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            var byteArray = Encoding.ASCII.GetBytes("mehar:Rbh@123");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public async Task CreateUserAsync(object user)
        {
            var response = await _httpClient.PostAsJsonAsync($"http://localhost:5984/{_dbName}", user);
            response.EnsureSuccessStatusCode(); // throws if not 200 OK
        }
    }

    public class UserLoginService
    {
        private readonly HttpClient _httpClient;
        private readonly string _dbName = "chat_app_db";

        public UserLoginService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            var byteArray = Encoding.ASCII.GetBytes("mehar:Rbh@123");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            var query = new
            {
                selector = new
                {
                    Username = username
                }
            };

            var response = await _httpClient.PostAsJsonAsync($"http://localhost:5984/{_dbName}/_find", query);
            if (!response.IsSuccessStatusCode)
                return null;
            
            Console.WriteLine(response);

            var result = await response.Content
                .ReadFromJsonAsync<CouchDbQueryResult<User>>();
            
            Console.WriteLine(result);
            Console.WriteLine(result?.Docs?.FirstOrDefault());

            return result?.Docs?.FirstOrDefault();
        }

        public async Task UpdateUserPassword(string userId, string hashedPassword)
        {
            var response = await _httpClient.GetAsync($"http://localhost:5984/{_dbName}/{userId}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<User>(json, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

            // Update password
            user.Password = hashedPassword;

            // 3️⃣ Send updated doc back (must include _rev)
            var content = new StringContent(
                JsonSerializer.Serialize(user),
                Encoding.UTF8,
                "application/json"
            );

            var putResponse = await _httpClient.PutAsync($"http://localhost:5984/{_dbName}/{userId}", content);
            putResponse.EnsureSuccessStatusCode();
        }
    }
}

