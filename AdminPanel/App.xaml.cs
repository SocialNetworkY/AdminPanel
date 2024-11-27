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
        services.AddSingleton<BanService>();
        services.AddTransient<AuthService>();
        services.AddSingleton<PostService>();
        services.AddSingleton<ReportService>();
        
        services.AddTransient<LoginViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<MainPageViewModel>();
        services.AddTransient<UsersPageViewModel>();
        
        services.AddSingleton<MainContainer>();
        services.AddSingleton<LoginUserControl>(provider => new LoginUserControl {
            DataContext = provider.GetRequiredService<LoginViewModel>()
        });
        services.AddSingleton<MainUserControl>(provider => new MainUserControl() {
            DataContext = provider.GetRequiredService<MainViewModel>()
        });
        services.AddSingleton<MainPage>(provider => new MainPage() {
            DataContext = provider.GetRequiredService<MainPageViewModel>()
        });
        services.AddSingleton<ReportsPage>(provider => new ReportsPage() {
            
        });
        services.AddSingleton<UsersPage>(provider => new UsersPage() {
            DataContext = provider.GetRequiredService<UsersPageViewModel>()
        });
        services.AddSingleton<PostsPage>(provider => new PostsPage() {
            
        });
        services.AddSingleton<CommentsPage>(provider => new CommentsPage() {
            
        });
        services.AddSingleton<BansPage>(provider => new BansPage() {
            
        });
        
        _serviceProvider = services.BuildServiceProvider();
    }

    protected override async void OnStartup(StartupEventArgs e) {
        try {
            if (await _serviceProvider.GetRequiredService<AuthService>().IsAuthenticated()) {
                _serviceProvider.GetRequiredService<NavigationService>().Navigate(_serviceProvider.GetRequiredService<MainUserControl>());
            }
            else {
                _serviceProvider.GetRequiredService<NavigationService>().Navigate(_serviceProvider.GetRequiredService<LoginUserControl>());
            }
            
            _serviceProvider.GetRequiredService<MainContainer>().Show();
            base.OnStartup(e);
        }
        catch (Exception ex) {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}