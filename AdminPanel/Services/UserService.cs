using System.Net.Http;
using System.Net.Http.Json;
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
}