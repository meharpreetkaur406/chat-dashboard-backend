using System.Net.Http.Headers;
using System.Text;
using ChatDashboard.Api.Models;
using System.Text.Json;

namespace ChatDashboard.Api.Services
{
    public class AdminService
    {
        private readonly HttpClient _httpClient;
        private readonly string _dbName = "chat_app_db";

        public AdminService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            var byteArray = Encoding.ASCII.GetBytes("mehar:Rbh@123");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public async Task<string> GetAllPendingUsers()
        {
            var query = new
            {
                selector = new
                {
                    type = "user",
                    status = "pending"
                }
            };

            var json = JsonSerializer.Serialize(query);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"http://localhost:5984/{_dbName}/_find", content);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task ApproveUserAsync(string id, string role)
        {
            var getResponse = await _httpClient.GetAsync($"http://localhost:5984/{_dbName}/{id}");

            if (!getResponse.IsSuccessStatusCode)
                throw new Exception("User not found in CouchDB");

            var jsonString = await getResponse.Content.ReadAsStringAsync();

            var doc = JsonSerializer.Deserialize<UserDocument>(jsonString);

            if (doc == null)
                throw new Exception("Failed to deserialize user");

            doc.role = role.ToLower();
            doc.status = "approved";
            doc.requestedRole = null;

            var updatedJson = JsonSerializer.Serialize(doc);
            var content = new StringContent(updatedJson, Encoding.UTF8, "application/json");

            var putResponse = await _httpClient.PutAsync(
                $"http://localhost:5984/{_dbName}/{doc._id}",
                content
            );

            if (!putResponse.IsSuccessStatusCode)
                throw new Exception("Failed to update user in CouchDB");
        }
    }
}