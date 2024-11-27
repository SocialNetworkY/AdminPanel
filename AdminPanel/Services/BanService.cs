using System.Net.Http;
using System.Net.Http.Json;
using AdminPanel.Generic.Services;
using AdminPanel.Models;

namespace AdminPanel.Services;

public class BanService(HttpClient httpClient, ISessionManager sessionManager) : BaseService(httpClient, sessionManager) {
    public async Task<BansStatistic?> GetBansStatisticAsync() {
        var request = new HttpRequestMessage(HttpMethod.Get, "user/api/v1/admin/bans/stats");
        var response = await SendRequestAsync(request);
        if (!response.IsSuccessStatusCode) {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<BansStatistic>();
    }
}