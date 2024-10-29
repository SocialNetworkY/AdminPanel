using System.Windows;
using System.Windows.Input;
using AdminPanel.Generic.Services;
using AdminPanel.Generic.ViewModels;
using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;

namespace AdminPanel.ViewModels;

public class LoginViewModel(AuthService authService, UserService userService, NavigationService navigationService) : ViewModelBase {
    private string _login = String.Empty;
    private bool _isLoggingIn;

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
            if (!await authService.LoginAsync(Login, password)) {
                MessageBox.Show("Invalid login or password");
                return;
            }
        
            var user = await userService.GetFullUserInfoAsync(await userService.GetMyIdAsync());
            if (user == null) {
                MessageBox.Show("Failed to get user info");
                return;
            }

            if (!user.IsAdmin) {
                MessageBox.Show("You are not an admin");
                return;
            }
        
            MessageBox.Show($"Login successful as {user.Username}");
            navigationService.Navigate(new MainUserControl());
        }
        finally {
            IsLoggingIn = false;
        }
    }
}