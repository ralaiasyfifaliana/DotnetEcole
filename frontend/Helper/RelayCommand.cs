namespace GestionHospitaliere.Frontend.ViewModels;

using System;
using System.Threading.Tasks;
using System.Windows.Input;

public class RelayCommand : ICommand
{
    private readonly Func<object?, Task>? _asyncExecute;
    private readonly Action<object?>? _execute;
    private readonly Predicate<object?>? _canExecute;

    public event EventHandler? CanExecuteChanged;

    // --- CONSTRUCTEURS SANS PARAMÈTRE ---
    public RelayCommand(Action execute, Func<bool>? canExecute = null)
    {
        _execute = _ => execute();
        if (canExecute != null) _canExecute = _ => canExecute();
    }

    public RelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _asyncExecute = _ => execute();
        if (canExecute != null) _canExecute = _ => canExecute();
    }

    // --- CONSTRUCTEURS AVEC PARAMÈTRE (object?) ---
    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public RelayCommand(Func<object?, Task> execute, Predicate<object?>? canExecute = null)
    {
        _asyncExecute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    public async void Execute(object? parameter)
    {
        if (_asyncExecute != null)
        {
            await _asyncExecute(parameter);
        }
        else
        {
            _execute?.Invoke(parameter);
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
public class RelayCommand<T> : ICommand
{
    private readonly Func<T?, Task>? _asyncExecute;
    private readonly Action<T?>? _execute;
    private readonly Predicate<T?>? _canExecute;

    public event EventHandler? CanExecuteChanged;

    public RelayCommand(Action<T?> execute, Predicate<T?>? canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public RelayCommand(Func<T?, Task> execute, Predicate<T?>? canExecute = null)
    {
        _asyncExecute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) => _canExecute?.Invoke(ConvertParameter(parameter)) ?? true;

    public async void Execute(object? parameter)
    {
        var typedParameter = ConvertParameter(parameter);

        if (_asyncExecute != null)
        {
            await _asyncExecute(typedParameter);
        }
        else
        {
            _execute?.Invoke(typedParameter);
        }
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    // Si le CommandParameter passé par Avalonia n'est pas du bon type (ex: null),
    // on retombe sur default(T) au lieu de planter.
    private static T? ConvertParameter(object? parameter) => parameter is T typed ? typed : default;
}