using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AdminPanel.Services;

public class TokenService {
    private readonly HttpClient _httpClient;
    private readonly CookieContainer _cookieContainer = new();
    private string? _accessToken = string.Empty;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    
    public TokenService(HttpClient httpClient) {
        var handler = new HttpClientHandler {
            CookieContainer = _cookieContainer,
            UseCookies = true,
            UseDefaultCredentials = false,
            AllowAutoRedirect = false
        };
        _httpClient = new HttpClient(handler) {
            BaseAddress = httpClient.BaseAddress
        };
    }
    
    public AuthenticationHeaderValue AuthorizationHeader => new("Bearer", _accessToken);

    public async Task<bool> LoginAsync(string login, string password) {
        await _mutex.WaitAsync();
        try {
            var response = await _httpClient.PostAsJsonAsync("/auth/api/v1/login", new { login, password });
            if (!response.IsSuccessStatusCode) {
                return false;
            }
            
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            _accessToken = tokenResponse!.Token;
            return true;
        } finally {
            _mutex.Release();
        }
        
    }
    
    public async Task<bool> RefreshAsync() {
        await _mutex.WaitAsync();
        try {
            var response = await _httpClient.PostAsync("/auth/api/v1/refresh", null);
            if (!response.IsSuccessStatusCode) {
                return false;
            }
            
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            _accessToken = tokenResponse!.Token;
            return true;
        } finally {
            _mutex.Release();
        }
    }
    
    public async Task WaitReadyAsync() {
        await _mutex.WaitAsync();
        _mutex.Release();
    }
}

public class TokenResponse {
    public string? Token { get; set; }
}