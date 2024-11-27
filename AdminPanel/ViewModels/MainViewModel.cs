using System.Windows.Controls;
using AdminPanel.Generic.ViewModels;
using AdminPanel.Views;

namespace AdminPanel.ViewModels;

public class MainViewModel(ReportsPage reportsPage, MainPage mainPage, UsersPage usersPage, PostsPage postsPage, CommentsPage commentsPage, BansPage bansPage): ViewModelBase {
    private string _currentPage;
    public string CurrentPage {
        get => _currentPage;
        set {
            if (_currentPage != value) {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(SelectedView));
            }
        }
    }

    public UserControl SelectedView {
        get {
            switch (CurrentPage) {
                case "Reports":
                    return reportsPage;
                case "Main":
                    return mainPage;
                case "Bans":
                    return bansPage;
                case "Users":
                    return usersPage;
                case "Posts":
                    return postsPage;
                case "Comments":
                    return commentsPage;
                default:
                    return mainPage;
            }
        }
    }
}