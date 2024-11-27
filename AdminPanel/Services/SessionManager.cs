using System.Collections;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using AdminPanel.Generic.Services;

namespace AdminPanel.Services;

public class SessionManager : ISessionSetter, ISessionManager {
    private readonly HttpClient _httpClient;
    private readonly CookieContainer _cookieContainer = new();
    private string _accessToken = string.Empty;
    private string _oauthToken = string.Empty;
    private bool _isOAuth = false;
    private readonly SemaphoreSlim _mutex = new(1, 1);
    private const string TokenFile = "tokens.json";
    
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

        LoadTokens();
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
            SaveTokens();
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
        try {
            if (isOAuth) {
                _oauthToken = token;
            } else {
                _accessToken = token;
            }
            _isOAuth = isOAuth;
            SaveTokens();
        }
        finally {
            _mutex.Release();    
        }
    }
    
    public async Task SetCookieAsync(string cookieHeader) {
        await _mutex.WaitAsync();
        try {
            _cookieContainer.SetCookies(_httpClient.BaseAddress!, cookieHeader);
            SaveTokens();
        }
        finally {
            _mutex.Release();    
        }
    }

    private void LoadTokens() {
        if (File.Exists(TokenFile)) {
            var tokens = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(TokenFile));
            if (tokens is null) {
                return;
            }
            if (tokens["access_token"] is JsonElement newAccessToken ) {
                _accessToken = newAccessToken.GetString()!;   
            }
            if (tokens["oauth_token"] is JsonElement newOAuthToken) {
                _oauthToken = newOAuthToken.GetString()!;   
            }

            _isOAuth = _oauthToken != "";
            
            if (tokens["cookies"] is JsonElement cookiesElement && 
                cookiesElement.ValueKind == JsonValueKind.Array) {
                var cookiesData = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(cookiesElement.GetRawText());
                if (cookiesData != null) {
                    _cookieContainer.Add(CookieCollectionExtensions.FromSerializableList(cookiesData));
                }
            }
        }
    }

    private void SaveTokens() {
        var tokens = new Dictionary<string, object> {
            { "access_token", _accessToken },
            { "oauth_token", _oauthToken },
            { "cookies", _cookieContainer.GetAllCookies().ToSerializableList() },
        };
        File.WriteAllText(TokenFile, JsonSerializer.Serialize(tokens));
    }
}

public static class CookieCollectionExtensions {
    public static List<Dictionary<string, string>> ToSerializableList(this CookieCollection cookies) {
        return cookies.Cast<Cookie>().Select(cookie => new Dictionary<string, string> {
            { "Name", cookie.Name },
            { "Value", cookie.Value },
            { "Path", cookie.Path },
            { "Domain", cookie.Domain },
            { "Secure", cookie.Secure.ToString() },
            { "HttpOnly", cookie.HttpOnly.ToString() },
            { "Expires", cookie.Expires.ToString("o") } // Используем ISO формат для даты
        }).ToList();
    }

    public static CookieCollection FromSerializableList(List<Dictionary<string, string>> cookiesData) {
        var cookieCollection = new CookieCollection();
        foreach (var cookieData in cookiesData) {
            var cookie = new Cookie {
                Name = cookieData["Name"],
                Value = cookieData["Value"],
                Path = cookieData["Path"],
                Domain = cookieData["Domain"],
                Secure = bool.Parse(cookieData["Secure"]),
                HttpOnly = bool.Parse(cookieData["HttpOnly"]),
                Expires = DateTime.TryParse(cookieData["Expires"], out var expires) ? expires : DateTime.MinValue
            };
            cookieCollection.Add(cookie);
        }
        return cookieCollection;
    }
}

public static class CookieContainerExtensions {
    public static CookieCollection GetAllCookies(this CookieContainer cookieContainer) {
        var cookies = new CookieCollection();
        var table = (Hashtable)typeof(CookieContainer)
            .GetField("m_domainTable", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(cookieContainer)!;

        foreach (var key in table.Keys) {
            var domain = key as string;
            if (table[key] is not null) {
                var type = table[key].GetType();
                var values = type.GetField("m_list", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .GetValue(table[key]) as IDictionary;
                if (values is null) continue;

                foreach (var val in values.Values) {
                    var cookieCollection = val as CookieCollection;
                    cookies.Add(cookieCollection);
                }
            }
        }
        return cookies;
    }
}
