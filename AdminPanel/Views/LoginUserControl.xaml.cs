using System.Windows.Controls;
using AdminPanel.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AdminPanel.Views;

public partial class LoginUserControl : UserControl
{
    public LoginUserControl()
    {
        InitializeComponent();
        DataContext = App.Services.GetRequiredService<LoginViewModel>();
    }
}