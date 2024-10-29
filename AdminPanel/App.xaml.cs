using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using AdminPanel.Generic.Services;
using AdminPanel.Services;
using AdminPanel.ViewModels;
using AdminPanel.Views;

namespace AdminPanel;

public partial class App : Application {
    private static IServiceProvider? _serviceProvider;
    
    public App() {
        IServiceCollection services = new ServiceCollection();
        
        services.AddSingleton(new HttpClient {
            BaseAddress = new Uri("http://localhost")
        });
        
        services.AddSingleton<NavigationService>(provider => new NavigationService(provider.GetRequiredService<MainContainer>().MainContentControl));
        services.AddSingleton<SessionManager>();
        services.AddSingleton<ISessionManager>(provider => provider.GetRequiredService<SessionManager>());
        services.AddSingleton<ISessionSetter>(provider => provider.GetRequiredService<SessionManager>());
        services.AddTransient<UserService>();
        services.AddTransient<AuthService>();
        
        services.AddTransient<LoginViewModel>();
        
        services.AddSingleton<MainContainer>();
        services.AddSingleton<LoginUserControl>(provider => new LoginUserControl {
            DataContext = provider.GetRequiredService<LoginViewModel>()
        });
        
        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e) {
        _serviceProvider.GetRequiredService<NavigationService>().Navigate(_serviceProvider.GetRequiredService<LoginUserControl>());
        _serviceProvider.GetRequiredService<MainContainer>().Show();
        base.OnStartup(e);
    }
}