using System.Net.Http;
using System.Net.Http.Json;
using AdminPanel.Generic.Services;

namespace AdminPanel.Services;

public class AuthService(HttpClient httpClient, ISessionSetter sessionManager) {
    public async Task<bool> LoginAsync(string login, string password) {
        var response = await httpClient.PostAsJsonAsync("/auth/api/v1/login", new { login, password });
        if (!response.IsSuccessStatusCode) {
            return false;
        }
        var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        await sessionManager.SetAccessTokenAsync(responseData!["token"]);
        
        foreach (var cookieHeader in response.Headers.GetValues("Set-Cookie")) {
            await sessionManager.SetCookieAsync(cookieHeader);
        }
        return true;
    }
}