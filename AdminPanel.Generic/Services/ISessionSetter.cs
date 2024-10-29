namespace AdminPanel.Generic.Services;

public interface ISessionSetter {
    Task SetAccessTokenAsync(string token, bool isOAuth = false);
    Task SetCookieAsync(string cookieHeader);
}