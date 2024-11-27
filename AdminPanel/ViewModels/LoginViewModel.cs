using System.Windows;
using System.Windows.Input;
using AdminPanel.Generic.Services;
using AdminPanel.Generic.ViewModels;
using AdminPanel.Models;
using AdminPanel.Services;
using AdminPanel.Views;
using Microsoft.Extensions.DependencyInjection;

namespace AdminPanel.ViewModels;

public class LoginViewModel(AuthService authService, UserService userService, NavigationService navigationService, MainUserControl mainUserControl) : ViewModelBase {
    private string _login = String.Empty;
    private bool _isLoggingIn;
    private string _errMessage = String.Empty;

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

    public string ErrMessage {
        get => _errMessage;
         set {
             _errMessage = value;
             OnPropertyChanged(nameof(ErrMessage));
         }
    }

    public bool IsLoginEnabled => !IsLoggingIn;

    public async Task LoginAsync(string password) {
        ErrMessage = "";
        if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(password)) {
            ErrMessage = "Login and password are required";
            return;
        }
        
        IsLoggingIn = true;
        try {
            if (!await authService.LoginAsync(Login, password)) {
                ErrMessage = "Invalid login or password";
                return;
            }

            var user = await userService.GetFullUserInfoAsync(await userService.GetMyIdAsync());
            if (user == null) {
                ErrMessage = "Failed to get user info";
                return;
            }

            if (!user.IsAdmin) {
                ErrMessage = "You are not an admin";
                return;
            }

            ErrMessage = $"Login successful as {user.Username}";
            await Task.Delay(2000);
            navigationService.Navigate(mainUserControl);
        }
        catch (Exception ex) {
            ErrMessage = $"Error: {ex.Message}";
        }
        finally {
            IsLoggingIn = false;
        }
    }
}