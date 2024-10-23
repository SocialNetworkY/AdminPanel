using System.Windows;
using System.Windows.Controls;
using AdminPanel.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AdminPanel.Views;

public partial class LoginUserControl : UserControl
{
    public LoginUserControl()
    {
        InitializeComponent();
    }

    private async void LoginBtn_OnClick(object sender, RoutedEventArgs e) {
        if (DataContext is LoginViewModel viewModel) {
            await viewModel.LoginAsync(PasswordBox.Password);
        }
    }
}