﻿using System.Net.Http;
using System.Net.Http.Json;
using AdminPanel.Generic.Services;
using AdminPanel.Models;

namespace AdminPanel.Services;

public class PostService(HttpClient httpClient, ISessionManager sessionManager) : BaseService(httpClient, sessionManager) {
    public async Task<PostsStatistic?> GetPostsStatisticAsync() {
        var request = new HttpRequestMessage(HttpMethod.Get, "report/api/v1/reports/stats");
        var response = await SendRequestAsync(request);
        if (!response.IsSuccessStatusCode) {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<PostsStatistic>();
    }
}