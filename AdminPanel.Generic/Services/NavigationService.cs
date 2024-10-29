using System.Windows.Controls;

namespace AdminPanel.Generic.Services;

public class NavigationService(ContentControl contentControl) {
    public void Navigate(UserControl userControl) {
        contentControl.Content = userControl;
    }
}