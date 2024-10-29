using System.Net.Http.Headers;

namespace AdminPanel.Generic.Services;

public interface ISessionManager {
    Task<bool> RefreshAsync();
    Task WaitReadyAsync();
    bool IsBusy { get; }
    AuthenticationHeaderValue AuthorizationHeader { get; }
}