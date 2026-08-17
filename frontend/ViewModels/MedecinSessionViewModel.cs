namespace GestionHospitaliere.Frontend.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Models;

public class ConsultationItemViewModel : INotifyPropertyChanged
{
    private Consultation _consultation;
    private string _nomPatient = "";

    public Consultation Consultation 
    { 
        get => _consultation; 
        set { _consultation = value; OnPropertyChanged(); } 
    }

    public string NomPatient 
    { 
        get => _nomPatient; 
        set { _nomPatient = value; OnPropertyChanged(); } 
    }

    public int? RefCons => _consultation?.RefCons;
    public DateTime? Date => _consultation?.Date;
    public string? Type => _consultation?.Type;

    public ConsultationItemViewModel(Consultation consultation, string nomPatient)
    {
        _consultation = consultation;
        _nomPatient = nomPatient;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public class MedecinSessionViewModel : INotifyPropertyChanged
{
    private readonly MainWindowViewModel _mainViewModel;
    private readonly IMedecinServices _medecinServices;
    private readonly IPatientServices _patientServices;
    private readonly IConsultationServices _consultationServices;
    private readonly IChambreServices _chambreServices;

    private Medecin? _medecinConnecte;
    private string _loginMedecin = "";
    private string _posteMedecin = "";
    
    // Gestion des Onglets
    private bool _isPlanningTabActive = true;
    private bool _isConsultationTabActive = false;
    private bool _hasConsultationEnCours = false;

    private ObservableCollection<ConsultationItemViewModel> _consultationsList = new();
    private DateTimeOffset? _searchDate;
    
    // Report
    private DateOnly? _reportDate = DateOnly.FromDateTime(DateTime.Today);
    private TimeSpan? _reportTime = DateTime.Now.TimeOfDay;
    private Consultation? _consultationAReporter;

    // Consultation en cours (Champs Panneau P & C)
    private Consultation? _consultationEnCours;
    private Patient? _patientEnCours;

    // Champs Panneau P (Patient)
    private string _etatPatient = "Non-Hospitalisé";
    private DateTimeOffset? _dateHospOffset;
    private DateTimeOffset? _dateSortieOffset;
    private int? _numChambre = null;

    // Options pour les ComboBoxes
    public ObservableCollection<string> EtatsPatientsList { get; } = new()
    {
        "Non-Hospitalisé",
        "Hospitalisé",
    };

    public ObservableCollection<int> ChambresList { get; } = new();

    // Champs Panneau C (Consultation)
    private int _fraisConsultation;
    private string _objetFacture = "";
    private string _prescription = "";
    private bool _isFacturationActive = false;

    // Messages de notification et d'erreur
    private string _notificationMessage = "";
    private string _errorMessage = "";

    // Propriétés Onglets
    public bool IsPlanningTabActive 
    { 
        get => _isPlanningTabActive; 
        set { _isPlanningTabActive = value; OnPropertyChanged(); } 
    }
    
    public bool IsConsultationTabActive 
    { 
        get => _isConsultationTabActive; 
        set { _isConsultationTabActive = value; OnPropertyChanged(); } 
    }

    public bool HasConsultationEnCours 
    { 
        get => _hasConsultationEnCours; 
        set { _hasConsultationEnCours = value; OnPropertyChanged(); } 
    }

    // Propriétés Globales
    public string LoginMedecin { get => _loginMedecin; set { _loginMedecin = value; OnPropertyChanged(); } }
    public string PosteMedecin { get => _posteMedecin; set { _posteMedecin = value; OnPropertyChanged(); } }
    public ObservableCollection<ConsultationItemViewModel> ConsultationsList { get => _consultationsList; set { _consultationsList = value; OnPropertyChanged(); } }
    public DateTimeOffset? SearchDate { get => _searchDate; set { _searchDate = value; OnPropertyChanged(); } }
    
    // Report
    public DateTimeOffset? ReportDateOffset
    {
        get => _reportDate.HasValue ? new DateTimeOffset(_reportDate.Value.ToDateTime(TimeOnly.MinValue)) : null;
        set { _reportDate = value.HasValue ? DateOnly.FromDateTime(value.Value.Date) : null; OnPropertyChanged(); }
    }
    public TimeSpan? ReportTime { get => _reportTime; set { _reportTime = value; OnPropertyChanged(); } }
    public Consultation? ConsultationAReporter { get => _consultationAReporter; set { _consultationAReporter = value; OnPropertyChanged(); } }

    // Objets Consultation/Patient
    public Consultation? ConsultationEnCours { get => _consultationEnCours; set { _consultationEnCours = value; OnPropertyChanged(); } }
    public Patient? PatientEnCours { get => _patientEnCours; set { _patientEnCours = value; OnPropertyChanged(); } }

    // Binding Panneau P
    public string EtatPatient { get => _etatPatient; set { _etatPatient = value; OnPropertyChanged(); } }
    public DateTimeOffset? DateHospOffset { get => _dateHospOffset; set { _dateHospOffset = value; OnPropertyChanged(); } }
    public DateTimeOffset? DateSortieOffset { get => _dateSortieOffset; set { _dateSortieOffset = value; OnPropertyChanged(); } }
    public int? NumChambre { get => _numChambre; set { _numChambre = value; OnPropertyChanged(); } }

    // Binding Panneau C
    public int FraisConsultation { get => _fraisConsultation; set { _fraisConsultation = value; OnPropertyChanged(); } }
    public string ObjetFacture { get => _objetFacture; set { _objetFacture = value; OnPropertyChanged(); } }
    public string Prescription { get => _prescription; set { _prescription = value; OnPropertyChanged(); } }
    public bool IsFacturationActive { get => _isFacturationActive; set { _isFacturationActive = value; OnPropertyChanged(); } }

    // Notifications et Gestion Erreurs
    public string NotificationMessage { get => _notificationMessage; set { _notificationMessage = value; OnPropertyChanged(); } }
    
    public string ErrorMessage 
    { 
        get => _errorMessage; 
        set 
        { 
            _errorMessage = value; 
            OnPropertyChanged(); 
            OnPropertyChanged(nameof(HasError)); 
        } 
    }

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    // Commandes
    public ICommand DeconnexionCommand { get; }
    public ICommand AfficherPlanningTabCommand { get; }
    public ICommand AfficherConsultationTabCommand { get; }
    public ICommand ChercherDateCommand { get; }
    public ICommand ReinitialiserRechercheCommand { get; }
    public ICommand FiltreDemainCommand { get; }
    public ICommand Filtre3JoursCommand { get; }
    public ICommand FiltreAchevesCommand { get; }
    public ICommand PreparerReportCommand { get; }
    public ICommand ValiderReportCommand { get; }
    public ICommand AnnulerReportCommand { get; }
    public ICommand ConsulterCommand { get; }
    public ICommand ActiverFacturationCommand { get; }
    public ICommand AnnulerFacturationCommand { get; }
    public ICommand TerminerConsultationCommand { get; }

    public MedecinSessionViewModel(
        string login, 
        MainWindowViewModel mainViewModel, 
        IMedecinServices medecinServices, 
        IPatientServices patientServices, 
        IConsultationServices consultationServices,
        IChambreServices chambreServices)
    {
        _mainViewModel = mainViewModel;
        _medecinServices = medecinServices;
        _patientServices = patientServices;
        _consultationServices = consultationServices;
        _chambreServices = chambreServices;
        LoginMedecin = login;

        DeconnexionCommand = new RelayCommand(() => _mainViewModel.NaviguerVers(new LoginViewModel(null!, _mainViewModel, null!, null!, null!, null!)));
        
        AfficherPlanningTabCommand = new RelayCommand(() => { IsPlanningTabActive = true; IsConsultationTabActive = false; });
        AfficherConsultationTabCommand = new RelayCommand(() => { if (HasConsultationEnCours) { IsPlanningTabActive = false; IsConsultationTabActive = true; } });

        ChercherDateCommand = new RelayCommand(async () => await ChargerConsultations(1));
        ReinitialiserRechercheCommand = new RelayCommand(async () => { SearchDate = null; await ChargerConsultations(0); });
        FiltreDemainCommand = new RelayCommand(async () => await ChargerConsultations(2));
        Filtre3JoursCommand = new RelayCommand(async () => await ChargerConsultations(3));
        FiltreAchevesCommand = new RelayCommand(async () => await ChargerConsultations(4));
        
        PreparerReportCommand = new RelayCommand<ConsultationItemViewModel>(vm => ConsultationAReporter = vm?.Consultation);
        AnnulerReportCommand = new RelayCommand(() => ConsultationAReporter = null);
        ValiderReportCommand = new RelayCommand(async () => await ReporterConsultation());
        
        ConsulterCommand = new RelayCommand<ConsultationItemViewModel>(async vm => await DemarrerConsultation(vm?.Consultation));
        ActiverFacturationCommand = new RelayCommand(() => IsFacturationActive = true);
        AnnulerFacturationCommand = new RelayCommand(() => { IsFacturationActive = false; ObjetFacture = ""; FraisConsultation = 0; ErrorMessage = ""; });
        TerminerConsultationCommand = new RelayCommand(async () => await TerminerConsultation());

        _ = InitialiserSession();
    }

    private async Task InitialiserSession()
    {
        await ChargerChambres();

        var medecins = await _medecinServices.ListeMedecin();
        _medecinConnecte = medecins.FirstOrDefault(m => m?.CodeMed == LoginMedecin);
        if (_medecinConnecte != null) 
        { 
            PosteMedecin = _medecinConnecte.Poste ?? "Médecin"; 
            await ChargerConsultations(0); 
        }
    }

    private async Task ChargerChambres()
    {
        var chambres = await _chambreServices.ListeChambre();
        ChambresList.Clear();
        foreach (var c in chambres)
        {
            if (c != null)
            {
                ChambresList.Add(c.NumChambre??0);
            }
        }
    }

    private async Task ChargerConsultations(int mode)
    {
        if (_medecinConnecte?.Idmed == null) return;
        List<Consultation?> query = mode switch
        {
            1 => (await _consultationServices.RechercheConsultation(null, SearchDate.HasValue ? DateOnly.FromDateTime(SearchDate.Value.Date) : null, null, _medecinConnecte.CodeMed, null)).ToList(),
            2 => (await _consultationServices.ListConsMedecin(_medecinConnecte.Idmed.Value, DateOnly.FromDateTime(DateTime.Today.AddDays(1)))).ToList(),
            3 => (await _consultationServices.RechercheConsultation(null, DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today.AddDays(3)), _medecinConnecte.CodeMed, null)).ToList(),
            4 => (await _consultationServices.ListConsAchevee(_medecinConnecte.Idmed.Value, DateOnly.FromDateTime(DateTime.Today))).ToList(),
            _ => (await _consultationServices.ListConsMedecin(_medecinConnecte.Idmed.Value, DateOnly.FromDateTime(DateTime.Today))).Where(c => string.IsNullOrEmpty(c?.ObjetFacture)).ToList()
        };

        var patients = (await _patientServices.ListePatients()).ToList();
        ConsultationsList.Clear();
        foreach (var c in query.Where(c => c != null))
        {
            var p = patients.FirstOrDefault(x => x?.IdPatient == c!.IdPatient);
            ConsultationsList.Add(new ConsultationItemViewModel(c!, p?.NomPatient ?? $"Patient N° {c!.IdPatient}"));
        }
    }

    private async Task ReporterConsultation()
    {
        if (ConsultationAReporter == null || _reportDate == null || ReportTime == null) return;
        
        DateTime nouvelleDate = _reportDate.Value.ToDateTime(TimeOnly.FromTimeSpan(ReportTime.Value));
        var tousLesPatients = await _patientServices.ListePatients();
        var patient = tousLesPatients.FirstOrDefault(p => p?.IdPatient == ConsultationAReporter.IdPatient);
        string? numPatient = patient?.NumPatient;

        var (success, message) = await _consultationServices.ModifierConsultation(
            ConsultationAReporter.RefCons,
            ConsultationAReporter.Type,
            nouvelleDate, 
            ConsultationAReporter.Frais,
            _medecinConnecte?.CodeMed,
            numPatient,
            ConsultationAReporter.Prescription,
            ConsultationAReporter.ObjetFacture,
            ConsultationAReporter.EtatFacture
        );

        if (success)
        {
            ConsultationAReporter = null;
            await ChargerConsultations(0);
        }
        else
        {
            NotificationMessage = message;
        }
    }

    private async Task DemarrerConsultation(Consultation? c)
    {
        if (c == null) return;
        
        ErrorMessage = "";
        ConsultationEnCours = c;
        var patients = await _patientServices.ListePatients();
        PatientEnCours = patients.FirstOrDefault(p => p?.IdPatient == c.IdPatient);

        // Initialisation Panneau P
        if (PatientEnCours != null)
        {
            EtatPatient = string.IsNullOrWhiteSpace(PatientEnCours.Etat) ? "Non-Hospitalisé" : PatientEnCours.Etat;
            DateHospOffset = PatientEnCours.DateHosp.HasValue ? new DateTimeOffset(PatientEnCours.DateHosp.Value.ToDateTime(TimeOnly.MinValue)) : null;
            DateSortieOffset = PatientEnCours.DateSortie.HasValue ? new DateTimeOffset(PatientEnCours.DateSortie.Value.ToDateTime(TimeOnly.MinValue)) : null;
            NumChambre = PatientEnCours.NumChambre;
        }

        // Initialisation Panneau C
        FraisConsultation = c.Frais;
        ObjetFacture = c.ObjetFacture ?? "";
        Prescription = c.Prescription ?? "";
        IsFacturationActive = !string.IsNullOrWhiteSpace(c.ObjetFacture);

        // Bascule vers l'onglet Consultation
        HasConsultationEnCours = true;
        IsPlanningTabActive = false;
        IsConsultationTabActive = true;
    }

    private async Task TerminerConsultation()
    {
        ErrorMessage = ""; // Réinitialiser les erreurs

        if (ConsultationEnCours == null || PatientEnCours == null) return;

        // Validation des règles métier si la facturation est active
        if (IsFacturationActive)
        {
            if (string.IsNullOrWhiteSpace(ObjetFacture))
            {
                ErrorMessage = "Veuillez préciser l'objet de la facture avant de terminer.";
                return;
            }

            if (FraisConsultation <= 0)
            {
                ErrorMessage = "Le montant des frais de facturation doit être défini et supérieur à 0 Ar.";
                return;
            }
        }

        // Cas : Aucune facturation activée -> Annulation et suppression de la consultation
        if (!IsFacturationActive)
        {
            await _consultationServices.AnnulerConsultation(ConsultationEnCours.RefCons??0);
            NotificationMessage = "Facturation non effectuée : la consultation a été annulée et le rendez-vous supprimé.";
        }
        else
        {
            // 1. Mise à jour des informations Patient (Panneau P)
            PatientEnCours.Etat = EtatPatient;
            PatientEnCours.DateHosp = DateHospOffset.HasValue ? DateOnly.FromDateTime(DateHospOffset.Value.Date) : null;
            PatientEnCours.DateSortie = DateSortieOffset.HasValue ? DateOnly.FromDateTime(DateSortieOffset.Value.Date) : null;
            PatientEnCours.NumChambre = NumChambre ?? null;
            await _patientServices.UpdatePatient(PatientEnCours.NumPatient??"", PatientEnCours.NomPatient, PatientEnCours.Etat, PatientEnCours.DateHosp, PatientEnCours.DateSortie, PatientEnCours.NumChambre);

            // 2. Mise à jour de la Consultation (Panneau C)
            DateTime dateCons = ConsultationEnCours.Date ?? DateTime.Now;
            string? numPatient = PatientEnCours.NumPatient;

            await _consultationServices.ModifierConsultation(
                ConsultationEnCours.RefCons,
                ConsultationEnCours.Type,
                dateCons,
                FraisConsultation,
                _medecinConnecte?.CodeMed,
                numPatient,
                Prescription,
                ObjetFacture,
                "Non-Payé"
            );

            // 3. Génération des documents
            GenererOrdonnancePDF();
            GenererFacturePDF();

            NotificationMessage = $"Consultation Réf: {ConsultationEnCours.RefCons} terminée avec succès. Documents générés.";
        }

        // Réinitialisation & Retour au planning
        HasConsultationEnCours = false;
        ConsultationEnCours = null;
        PatientEnCours = null;
        IsConsultationTabActive = false;
        IsPlanningTabActive = true;
        await ChargerConsultations(0);
    }

    private void GenererOrdonnancePDF()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Ordonnance_{ConsultationEnCours?.RefCons}.txt");
        string contenu = $"""
        ==================================================
        CENTRE D'HOSPITALISATION H#
        ==================================================
        ORDONNANCE N° : {ConsultationEnCours?.RefCons}
        Médecin : {LoginMedecin} (Code: {_medecinConnecte?.CodeMed})
        Patient : {PatientEnCours?.NomPatient} (Num: {PatientEnCours?.NumPatient})
        Date : {DateTime.Now:dd/MM/yyyy HH:mm}
        --------------------------------------------------
        PRESCRIPTION :
        {Prescription}
        ==================================================
        """;
        File.WriteAllText(path, contenu);
    }

    private void GenererFacturePDF()
    {
        string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Facture_{ConsultationEnCours?.RefCons}.txt");
        string contenu = $"""
        ==================================================
        CENTRE D'HOSPITALISATION H#
        ==================================================
        FACTURE N° : {ConsultationEnCours?.RefCons}
        Médecin : {LoginMedecin} (Code: {_medecinConnecte?.CodeMed})
        Patient : {PatientEnCours?.NomPatient} (Num: {PatientEnCours?.NumPatient})
        Date : {DateTime.Now:dd/MM/yyyy HH:mm}
        --------------------------------------------------
        OBJET : {ObjetFacture}
        MONTANT / FRAIS : {FraisConsultation} Ar
        --------------------------------------------------
        STATUT : A Régler
        ==================================================
        """;
        File.WriteAllText(path, contenu);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}