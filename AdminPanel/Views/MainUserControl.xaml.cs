using System.Windows;
using System.Windows.Controls;
using AdminPanel.ViewModels;

namespace AdminPanel.Views;

public partial class MainUserControl : UserControl {
    public MainUserControl() {
        InitializeComponent();
    }
    
    private void NavigateTo_Click(object sender, RoutedEventArgs e) {
        if (sender is not RadioButton button) return;
        ((MainViewModel)DataContext).CurrentPage = button.Tag.ToString();
    }
}
