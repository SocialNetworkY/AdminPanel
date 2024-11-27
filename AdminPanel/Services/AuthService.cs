using System.Net.Http;
using System.Net.Http.Json;
using AdminPanel.Generic.Services;

namespace AdminPanel.Services;

public class AuthService(HttpClient httpClient, ISessionSetter sessionSetter, ISessionManager sessionManager) : BaseService(httpClient, sessionManager)  {
    private readonly HttpClient _httpClient = httpClient;

    public async Task<bool> LoginAsync(string login, string password) {
        var response = await _httpClient.PostAsJsonAsync("/auth/api/v1/login", new { login, password });
        if (!response.IsSuccessStatusCode) {
            return false;
        }
        var responseData = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        await sessionSetter.SetAccessTokenAsync(responseData!["token"]);
        
        foreach (var cookieHeader in response.Headers.GetValues("Set-Cookie")) {
            await sessionSetter.SetCookieAsync(cookieHeader);
        }
        return true;
    }

    public async Task<bool> IsAuthenticated() {
        try {
            var request = new HttpRequestMessage(HttpMethod.Get, "/auth/api/v1/authenticate");
            var response = await SendRequestAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception e) {
            Console.WriteLine(e);
            return false;
        }
    }
}