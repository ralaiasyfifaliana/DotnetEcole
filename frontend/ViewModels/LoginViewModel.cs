namespace GestionHospitaliere.Frontend.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Models;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly IUsersServices _usersServices;
    private readonly IMedecinServices _medecinservices;
    private readonly IChambreServices _chambreServices;
    private readonly IPatientServices _patientsServices;
    private readonly IConsultationServices _consultationServices;
    private readonly MainWindowViewModel _mainViewModel;

    private string _login = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;

    public string Login
    {
        get => _login;
        set { _login = value; OnPropertyChanged(); }
    }

    public string Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set { _errorMessage = value; OnPropertyChanged(); }
    }

    public ICommand LoginCommand { get; }

    public LoginViewModel(
        IUsersServices usersServices,
        MainWindowViewModel mainViewModel,
        IMedecinServices medecinservices,
        IChambreServices chambreServices,
        IPatientServices patientsServices,
        IConsultationServices consultationServices)
    {
        _usersServices = usersServices;
        _mainViewModel = mainViewModel;
        _medecinservices = medecinservices;
        _chambreServices = chambreServices;
        _patientsServices = patientsServices;
        _consultationServices = consultationServices;

        LoginCommand = new RelayCommand(async () => await SeConnecterAsync());
    }

    private async Task SeConnecterAsync()
{
    ErrorMessage = string.Empty;
    Users? user = await _usersServices.SeConnecter(Login, Password);

    if (user != null)
    {
        if (user.Type == "Acceuil")
        {
            var dashboardVM = new DashboardViewModel(user, _mainViewModel, _usersServices, _medecinservices, _chambreServices, _patientsServices, _consultationServices);
            _mainViewModel.NaviguerVers(dashboardVM);
        }
        else if (user.Type == "medecin")
        {
            // Le login est utilisé comme CodeMed pour identifier le médecin
            var medecinVM = new MedecinSessionViewModel(Login, _mainViewModel, _medecinservices, _patientsServices, _consultationServices, _chambreServices);
            _mainViewModel.NaviguerVers(medecinVM);
        }
        else
        {
            ErrorMessage = $"Accès refusé. Le rôle '{user.Type}' n'est pas autorisé sur ce poste.";
        }
    }
    else
    {
        ErrorMessage = "Identifiant ou mot de passe incorrect.";
    }
}

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
