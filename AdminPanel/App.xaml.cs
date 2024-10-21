using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using AdminPanel.Services;
using AdminPanel.ViewModels;
using AdminPanel.Views;

namespace AdminPanel;

public partial class App : Application {
    public static IServiceProvider Services { get; private set; }

    protected override void OnStartup(StartupEventArgs e) {
        try {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            Services = serviceCollection.BuildServiceProvider();
        
            var mainContainer = Services.GetRequiredService<MainContainer>();
            var navigationService = Services.GetRequiredService<NavigationService>();
            mainContainer.SetNavigationService(navigationService);
            mainContainer.Show();
        }
        catch (Exception exception) {
            Console.WriteLine(exception);
            throw;
        }
    }
    
    private void ConfigureServices(IServiceCollection services) {
        var httpClient = new HttpClient {
            BaseAddress = new Uri("http://localhost")
        };
        services.AddSingleton(httpClient);
        services.AddSingleton<MainContainer>();
        services.AddSingleton<NavigationService>(provider => {
            var mainContainer = provider.GetRequiredService<MainContainer>();
            return new NavigationService(mainContainer.MainContentControl);
        });
        services.AddSingleton<TokenService>();
        services.AddTransient<UserService>();
        services.AddTransient<LoginViewModel>();
    }
}