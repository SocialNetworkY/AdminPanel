using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using AdminPanel.Generic.Services;

namespace AdminPanel.Services;

public class SessionManager : ISessionManager, ISessionSetter {
    private readonly HttpClient _httpClient;
    private readonly CookieContainer _cookieContainer = new();
    private string? _accessToken = string.Empty;
    private string? _oauthToken = string.Empty;
    private bool _isOAuth = false;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    
    public SessionManager(HttpClient httpClient) {
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
    
    public AuthenticationHeaderValue AuthorizationHeader => _isOAuth ? new AuthenticationHeaderValue("OAuth", _oauthToken) : new AuthenticationHeaderValue("Bearer", _accessToken);
    
    public async Task<bool> RefreshAsync() {
        await _mutex.WaitAsync();
        try {
            var response = await _httpClient.PostAsync("/auth/api/v1/refresh", null);
            if (!response.IsSuccessStatusCode) {
                return false;
            }
            
            var tokenResponse = await response.Content.ReadFromJsonAsync<Dictionary<string,string>>();
            if (_isOAuth) {
                _oauthToken = tokenResponse!["token"];
            } else {
                _accessToken = tokenResponse!["token"];
            }
            return true;
        } catch {
            return false;
        } finally {
            _mutex.Release();
        }
    }
    
    public async Task WaitReadyAsync() {
        await _mutex.WaitAsync();
        _mutex.Release();
    }
    
    public bool IsBusy => _mutex.CurrentCount == 0;
    
    public async Task SetAccessTokenAsync(string token, bool isOAuth = false) {
        await _mutex.WaitAsync();
        if (isOAuth) {
            _oauthToken = token;
        } else {
            _accessToken = token;
        }
        _isOAuth = isOAuth;
        _mutex.Release();
    }
    
    public async Task SetCookieAsync(string cookieHeader) {
        await _mutex.WaitAsync();
        try {
            _cookieContainer.SetCookies(_httpClient.BaseAddress!, cookieHeader);
        }
        finally {
            _mutex.Release();    
        }
    }
}