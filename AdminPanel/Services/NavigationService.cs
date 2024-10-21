using System.Windows.Controls;

namespace AdminPanel.Services;

public class NavigationService {
    private readonly ContentControl _contentControl;
    
    public NavigationService(ContentControl contentControl) {
        _contentControl = contentControl;
    }
    
    public void Navigate(UserControl userControl) {
        _contentControl.Content = userControl;
    }
}