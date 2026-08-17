using CommunityToolkit.Mvvm.ComponentModel;

namespace GestionHospitaliere.Frontend.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome!";
}
