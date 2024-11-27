using System.Windows.Input;
using AdminPanel.Generic.ViewModels;
using AdminPanel.Models;
using AdminPanel.Services;

namespace AdminPanel.ViewModels;

public class UsersPageViewModel: ViewModelBase {
    private readonly UserService _userService;
    private const int UpdateIntervalMilliseconds = 10000;
    private readonly CancellationTokenSource _cancellationTokenSource = new ();
    
    public UsersPageViewModel(UserService userService) {
        _userService = userService;
        BanUserCommand = new RelayCommand(async () => await BanUserAsync());
        UnbanUserCommand = new RelayCommand(async () => await UnbanUserAsync());
        
        Task.Run(BackgroundUpdateAsync, _cancellationTokenSource.Token);
    }
    
    private async Task BackgroundUpdateAsync() {
        while (!_cancellationTokenSource.Token.IsCancellationRequested) {
            try {
                await UpdateData();
                await Task.Delay(UpdateIntervalMilliseconds);
            }
            catch (TaskCanceledException) {
                Console.WriteLine("Loop canceled by token");
            }
            catch (Exception ex) {
                Console.WriteLine($"Background users update error: {ex.Message}");
            }   
        }
    }
    
    private async Task UpdateData() {
        var userStatistic = await _userService.GetUsersStatisticAsync();
        if (userStatistic != null) {
            this.UsersStatistic = userStatistic;
        }
        
        var users = await _userService.GetUsersAsync();
        if (users != null) {
            this.Users = users;
        }
    }
    
    private UsersStatistic _usersStatistic = new ();
    public UsersStatistic UsersStatistic {
        get => _usersStatistic;
        set {
            _usersStatistic = value;
            OnPropertyChanged(nameof(UsersStatistic));
        }
    }
    
    private List<User> _users = new();

    public List<User> Users {
        get => _users;
         set {
             _users = value;
             OnPropertyChanged(nameof(Users));
         }
    }
    
    private uint _banUserId;
    public uint BanUserId {
        get => _banUserId;
        set {
            _banUserId = value;
            OnPropertyChanged(nameof(BanUserId));
        }
    }

    private string _banReason = string.Empty;
    public string BanReason {
        get => _banReason;
        set {
            _banReason = value;
            OnPropertyChanged(nameof(BanReason));
        }
    }
    
    private string _banDuration = "e.g., 7d, 12h";
    public string BanDuration {
        get => _banDuration;
        set {
            _banDuration = value;
            OnPropertyChanged(nameof(BanDuration));
        }
    }

    private uint _unbanBanId;
    public uint UnbanBanId {
        get => _unbanBanId;
        set {
            _unbanBanId = value;
            OnPropertyChanged(nameof(UnbanBanId));
        }
    }

    private string _unbanReason = string.Empty;
    public string UnbanReason {
        get => _unbanReason;
        set {
            _unbanReason = value;
            OnPropertyChanged(nameof(UnbanReason));
        }
    }
    
    public ICommand BanUserCommand { get; }
    public ICommand UnbanUserCommand { get; }

    private async Task BanUserAsync() {
        if (BanUserId > 0 && !string.IsNullOrWhiteSpace(BanReason) && !string.IsNullOrWhiteSpace(BanDuration) && BanDuration != "e.g., 7d, 12h") {
            await _userService.BanUserAsync(BanUserId, BanReason, BanDuration);
            await UpdateData();
        }
    }

    private async Task UnbanUserAsync() {
        if (UnbanBanId > 0 && !string.IsNullOrWhiteSpace(UnbanReason)) {
            await _userService.UnbanUserAsync(UnbanBanId, UnbanReason);
            await UpdateData();
        }
    }
}