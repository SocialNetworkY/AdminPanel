using System.Net;
using System.Net.Http;

namespace AdminPanel.Services;

public abstract class BaseService {
    private readonly HttpClient _httpClient;
    protected readonly TokenService _tokenService;

    protected BaseService(HttpClient httpClient, TokenService tokenService) {
        _httpClient = httpClient;
        _tokenService = tokenService;
    }
    
    protected async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, uint attempts = 2, HttpResponseMessage? lastResponse = null) {
        if (attempts == 0) {
            return lastResponse!;
        }
        
        await _tokenService.WaitReadyAsync();
        request.Headers.Authorization = _tokenService.AuthorizationHeader;
        var response = await _httpClient.SendAsync(request);
        
        if (response.IsSuccessStatusCode) {
            return response;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized) {
            if (!await _tokenService.RefreshAsync()) {
                throw new Exception("Failed to Authenticate");
            }
            attempts += 1;
        }
        
        // Create a new request to avoid the disposed one
        var newRequest = new HttpRequestMessage(request.Method, request.RequestUri) {
            Content = request.Content,
            Version = request.Version
        };
        foreach (var header in request.Headers) {
            newRequest.Headers.Add(header.Key, header.Value);
        }
        
        return await SendRequestAsync(newRequest, attempts - 1, response);
    }
}