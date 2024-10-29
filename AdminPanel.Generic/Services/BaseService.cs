using System.Net;
using System.Net.Http;

namespace AdminPanel.Generic.Services;

public abstract class BaseService(HttpClient httpClient, ISessionManager sessionManager) {
    protected async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, uint attempts = 2, HttpResponseMessage? lastResponse = null) {
        if (attempts == 0) {
            return lastResponse!;
        }
        
        await sessionManager.WaitReadyAsync();
        request.Headers.Authorization = sessionManager.AuthorizationHeader;
        var response = await httpClient.SendAsync(request);
        
        if (response.IsSuccessStatusCode) {
            return response;
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized) {
            if (!sessionManager.IsBusy) {
                if (!await sessionManager.RefreshAsync()) {
                    throw new Exception("Failed to Authenticate");
                }
            } else {
                await sessionManager.WaitReadyAsync();
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