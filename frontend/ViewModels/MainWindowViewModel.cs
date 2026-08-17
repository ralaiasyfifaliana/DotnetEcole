namespace GestionHospitaliere.Frontend.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private object? _currentViewModel;

    public object? CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
            OnPropertyChanged();
        }
    }

    // Constructeur propre sans injection directe de LoginViewModel
    public MainWindowViewModel()
    {
    }

    public void NaviguerVers(object nouveauViewModel)
    {
        CurrentViewModel = nouveauViewModel;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}