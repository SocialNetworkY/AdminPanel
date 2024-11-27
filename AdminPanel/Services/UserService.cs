using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Windows;
using AdminPanel.Generic.Services;
using AdminPanel.Models;

namespace AdminPanel.Services;

public class UserService(HttpClient httpClient, ISessionManager sessionManager) : BaseService(httpClient, sessionManager) {
    public async Task<uint> GetMyIdAsync() {
        var request = new HttpRequestMessage(HttpMethod.Get, "auth/api/v1/info");
        var response = await SendRequestAsync(request);
        if (!response.IsSuccessStatusCode) {
            return 0;
        }
        var user = await response.Content.ReadFromJsonAsync<User>(); 
        return user!.Id;
    }
    
    public async Task<User?> GetUserInfoAsync(uint userId) {
        var request = new HttpRequestMessage(HttpMethod.Get, "user/api/v1/users/" + userId);
        var response = await SendRequestAsync(request);
        if (!response.IsSuccessStatusCode) {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<User>();
    }
    
    public async Task<User?> GetFullUserInfoAsync(uint userId) {
        var request = new HttpRequestMessage(HttpMethod.Get, "user/api/v1/users/" + userId + "/full");
        var response = await SendRequestAsync(request);
        if (!response.IsSuccessStatusCode) {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<User>();
    }

    public async Task<List<User>?> GetUsersAsync(uint skip = 0, uint size = 10) {
        var request = new HttpRequestMessage(HttpMethod.Get, "user/api/v1/admin/users?skip=" + skip + "&limit=" + size);
        var response = await SendRequestAsync(request);
        if (!response.IsSuccessStatusCode) {
            return new List<User>();
        }
        return await response.Content.ReadFromJsonAsync<List<User>>();
    }

    public async Task<UsersStatistic?> GetUsersStatisticAsync() {
        var request = new HttpRequestMessage(HttpMethod.Get, "user/api/v1/admin/users/stats");
        var response = await SendRequestAsync(request);
        if (!response.IsSuccessStatusCode) {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<UsersStatistic>();
    }

    public async Task BanUserAsync(uint userId, string reason, string duration) {
        var request = new HttpRequestMessage(HttpMethod.Post, "user/api/v1/admin/users/" + userId + "/ban");
        request.Content = new StringContent(JsonSerializer.Serialize(new { Reason = reason, Duration = duration }), Encoding.UTF8, "application/json");
        var response = await SendRequestAsync(request);
        if (!response.IsSuccessStatusCode) {
            MessageBox.Show("Error banning user: " + response.ReasonPhrase);
            return;
        }
        MessageBox.Show("User banned successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    
    public async Task UnbanUserAsync(uint banId, string reason) {
        var request = new HttpRequestMessage(HttpMethod.Post, "user/api/v1/admin/unban");
        request.Content = new StringContent(JsonSerializer.Serialize(new { BanID = banId, Reason = reason }), Encoding.UTF8, "application/json");
        var response = await SendRequestAsync(request);
        if (!response.IsSuccessStatusCode) {
            MessageBox.Show("Error unbanning user: " + response.ReasonPhrase);
            return;
        }
        MessageBox.Show("User unbanned successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}