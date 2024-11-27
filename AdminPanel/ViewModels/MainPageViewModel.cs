using System.Net.Mime;
using System.Windows;
using AdminPanel.Generic.ViewModels;
using AdminPanel.Services;

namespace AdminPanel.ViewModels;

public class MainPageViewModel : ViewModelBase {
    private readonly UserService _userService;
    private readonly BanService _banService;
    private readonly ReportService _reportService;
    private readonly PostService _postService;


    public MainPageViewModel(UserService userService, BanService banService, ReportService reportService, PostService postService) {
        _userService = userService;
        _banService = banService;
        _postService = postService;
        _reportService = reportService;
        PeriodicUpdateAsync();
    }
    
    private async Task PeriodicUpdateAsync() {
        while (true) {
            await UpdateData();
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
    
    private async Task UpdateData() {
        await Task.Run(async () => {
            var userStatistic = await _userService.GetUsersStatisticAsync();
            if (userStatistic != null) {
                Application.Current.Dispatcher.Invoke(() => UsersAmount = userStatistic.Total);
            }
            
            var banStatistic = await _banService.GetBansStatisticAsync();
            if (banStatistic != null) {
                Application.Current.Dispatcher.Invoke(() => BansAmount = banStatistic.Total);
            }
            
            var reportStatistic = await _reportService.GetReportsStatisticAsync();
            if (reportStatistic != null) {
                Application.Current.Dispatcher.Invoke(() => ReportsAmount = reportStatistic.Total);
            }

            var postStatistic = await _postService.GetPostsStatisticAsync();
            if (postStatistic != null) {
                Application.Current.Dispatcher.Invoke(() => PostsAmount = postStatistic.Total);
            }
        });
    }

    private uint _usersAmount;
    public uint UsersAmount {
        get => _usersAmount;
        set {
            if (_usersAmount != value) {
                _usersAmount = value;
                OnPropertyChanged(nameof(UsersAmount));
            }
        }
    }

    private uint _bansAmount;
    public uint BansAmount {
        get => _bansAmount;
        set {
            if (_bansAmount != value) {
                _bansAmount = value;
                OnPropertyChanged(nameof(BansAmount));
            }
        }
    }
    
    private uint _postsAmount;
    public uint PostsAmount {
        get => _postsAmount;
        set {
            if (_postsAmount != value) {
                _postsAmount = value;
                OnPropertyChanged(nameof(PostsAmount));
            }
        }
    }

    private uint _reportsAmount;
    public uint ReportsAmount {
        get => _reportsAmount;
        set {
            if (_reportsAmount != value) {
                _reportsAmount = value;
                OnPropertyChanged(nameof(ReportsAmount));
            }
        }
    }

}