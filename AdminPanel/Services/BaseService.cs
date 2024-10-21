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

        if (response.StatusCode == HttpStatusCode.Unauthorized && !await _tokenService.RefreshAsync()) {
            throw new Exception("Failed to Authenticate");
        }
        
        request.Headers.Authorization = _tokenService.AuthorizationHeader;
        response = await _httpClient.SendAsync(request);

        return await SendRequestAsync(request, attempts - 1, response);
    }
}