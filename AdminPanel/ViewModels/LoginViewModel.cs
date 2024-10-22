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
    private bool _isLoggingIn;
    
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
    
    public bool IsLoggingIn {
        get => _isLoggingIn;
        set {
            _isLoggingIn = value;
            OnPropertyChanged(nameof(IsLoggingIn));
            OnPropertyChanged(nameof(IsLoginEnabled));
        }
    }
    
    public bool IsLoginEnabled => !IsLoggingIn;

    public async Task LoginAsync(string password) {
        if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(password)) {
            MessageBox.Show("Login and password are required");
            return;
        }
        
        IsLoggingIn = true;
        try {
            if (!await _userService.LoginAsync(Login, password)) {
                MessageBox.Show("Invalid login or password");
                return;
            }
        
            var user = await _userService.GetFullUserInfoAsync(await _userService.GetMyIdAsync());
            if (user == null) {
                MessageBox.Show("Failed to get user info");
                return;
            }

            if (!user.IsAdmin) {
                MessageBox.Show("You are not an admin");
                return;
            }
        
            MessageBox.Show($"Login successful as {user.Username}");
            _navigationService.Navigate(new MainUserControl());
        }
        finally {
            IsLoggingIn = false;
        }
    }
}