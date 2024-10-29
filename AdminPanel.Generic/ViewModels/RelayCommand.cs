using System.Windows.Input;

namespace AdminPanel.Generic.ViewModels;

public class RelayCommand(Func<Task> execute, Func<bool>? canExecute = null) : ICommand {
    public event EventHandler? CanExecuteChanged;
    
    public bool CanExecute(object? parameter) => canExecute == null || canExecute();
    
    public async void Execute(object? parameter) => await execute();
    
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}