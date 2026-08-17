namespace GestionHospitaliere.Frontend.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Models;

public class DashboardViewModel : INotifyPropertyChanged
{
    private readonly Users _utilisateurConnecte;
    private readonly MainWindowViewModel _mainViewModel;
    private readonly IUsersServices _usersServices;
    private readonly IMedecinServices _medecinservices;
    private readonly IChambreServices _chambreServices;
    private readonly IPatientServices _patientsServices;
    private readonly IConsultationServices _consultationServices;

    private object? _currentSectionViewModel;
    private string _activeSection = "Dashboard";

    public Users UtilisateurConnecte => _utilisateurConnecte;

    public string ActiveSection
    {
        get => _activeSection;
        set
        {
            _activeSection = value;
            OnPropertyChanged();
        }
    }

    public object? CurrentSectionViewModel
    {
        get => _currentSectionViewModel;
        set
        {
            _currentSectionViewModel = value;
            OnPropertyChanged();
        }
    }

    // Commandes de navigation
    public ICommand NaviguerDashboardCommand { get; }
    public ICommand NaviguerConsultationsCommand { get; }
    public ICommand NaviguerMedecinsCommand { get; }
    public ICommand NaviguerPatientsCommand { get; }
    public ICommand NaviguerChambresCommand { get; }

    // Commande de déconnexion
    public ICommand DeconnexionCommand { get; }

    public DashboardViewModel(
        Users user,
        MainWindowViewModel mainViewModel,
        IUsersServices usersServices,
        IMedecinServices medecinservices,
        IChambreServices chambreServices,
        IPatientServices patientsServices,
        IConsultationServices consultationServices)
    {
        _utilisateurConnecte = user;
        _mainViewModel = mainViewModel;
        _usersServices = usersServices;
        _medecinservices = medecinservices;
        _chambreServices = chambreServices;
        _patientsServices = patientsServices;
        _consultationServices = consultationServices;

        // Commandes pour les sections
        NaviguerDashboardCommand = new RelayCommand(() => ChangerSection("Dashboard", null));
        NaviguerConsultationsCommand = new RelayCommand(() => ChangerSection("Consultations", new ConsultationsViewModel(_consultationServices, _medecinservices, _patientsServices)));
        NaviguerMedecinsCommand = new RelayCommand(() => ChangerSection("Medecins", new MedecinsViewModel(_medecinservices)));
        NaviguerPatientsCommand = new RelayCommand(() => ChangerSection("Patients", new PatientsViewModel(_patientsServices, _chambreServices)));

        // Instanciation de ChambresViewModel lors du clic
        NaviguerChambresCommand = new RelayCommand(() => ChangerSection("Chambres", new ChambresViewModel(_chambreServices)));

        // Commande pour se déconnecter
        DeconnexionCommand = new RelayCommand(SeDeconnecter);

        ChangerSection("Dashboard", null);
    }

    private void ChangerSection(string nomSection, object? viewModelSection)
    {
        ActiveSection = nomSection;
        CurrentSectionViewModel = viewModelSection;
    }

    private void SeDeconnecter()
    {
        var loginVM = new LoginViewModel(_usersServices, _mainViewModel, _medecinservices, _chambreServices, _patientsServices, _consultationServices);
        _mainViewModel.NaviguerVers(loginVM);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
