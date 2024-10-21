using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AdminPanel.Views;

namespace AdminPanel.Services;

public class TokenService {
    private readonly HttpClient _httpClient;
    private string _accessToken = string.Empty;
    private string _refreshToken = string.Empty;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    
    public TokenService(HttpClient httpClient) {
        _httpClient = httpClient;
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
            _accessToken = tokenResponse!.AccessToken;
            _refreshToken = tokenResponse.RefreshToken;
            return true;
        } finally {
            _mutex.Release();
        }
        
    }
    
    public async Task<bool> RefreshAsync() {
        await _mutex.WaitAsync();
        try {
            var response = await _httpClient.PostAsJsonAsync("/auth/api/v1/refresh", new { _refreshToken });
            if (!response.IsSuccessStatusCode) {
                return false;
            }
            
            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            _accessToken = tokenResponse!.AccessToken;
            _refreshToken = tokenResponse.RefreshToken;
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
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}