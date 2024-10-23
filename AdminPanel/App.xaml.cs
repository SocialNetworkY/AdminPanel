using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using AdminPanel.Services;
using AdminPanel.ViewModels;
using AdminPanel.Views;

namespace AdminPanel;

public partial class App : Application {
    private static IServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e) {
        try {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        
            var mainContainer = _serviceProvider.GetRequiredService<MainContainer>();
            var navigationService = _serviceProvider.GetRequiredService<NavigationService>();
            navigationService.Navigate(_serviceProvider.GetRequiredService<LoginUserControl>());
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
        services.AddSingleton<LoginUserControl>(provider => new LoginUserControl {
            DataContext = provider.GetRequiredService<LoginViewModel>()
        });
        services.AddSingleton<NavigationService>(provider => new NavigationService(provider.GetRequiredService<MainContainer>().MainContentControl));
        services.AddSingleton<TokenService>();
        services.AddTransient<UserService>();
        services.AddTransient<LoginViewModel>();
    }
}