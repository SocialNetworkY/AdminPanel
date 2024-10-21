using System.Windows;
using AdminPanel.Services;

namespace AdminPanel.Views;

public partial class MainContainer : Window {
    private NavigationService _navigationService;
    
    public MainContainer() {
        InitializeComponent();
    }
    
    public void SetNavigationService(NavigationService navigationService) {
        _navigationService = navigationService;
        _navigationService.Navigate(new LoginUserControl());
    }
}