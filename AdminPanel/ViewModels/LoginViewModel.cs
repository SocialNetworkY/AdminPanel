using System.Windows;
using System.Windows.Input;
using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;

namespace AdminPanel.ViewModels;

public class LoginViewModel: ViewModelBase
{
    private readonly UserService _userService;
    private readonly NavigationService _navigationService;
    private string _login = String.Empty;
    private string _password = String.Empty;
    public ICommand LoginCommand => new RelayCommand(async () => await LoginAsync());
    
    public LoginViewModel(UserService userService, NavigationService navigationService) {
        _userService = userService;
        _navigationService = navigationService;
    }
    public string Login {
        get => _login;
        set {
            _login = value;
            OnPropertyChanged(nameof(Login));
        }
    }

    public string Password {
        get => _password;
        set {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }

    private async Task LoginAsync() {
        if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password)) {
            MessageBox.Show("Login and password are required");
            return;
        }
        if (!await _userService.LoginAsync(Login, Password)) {
            MessageBox.Show("Invalid login or password");
            return;
        }
        MessageBox.Show("Login successful");
        _navigationService.Navigate(new MainUserControl());
    }
}