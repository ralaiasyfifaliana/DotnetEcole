namespace GestionHospitaliere.Frontend.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Models;

public class PatientsViewModel : INotifyPropertyChanged
{
    private readonly IPatientServices _patientServices;
    private readonly IChambreServices _chambreServices;

    // --- Notification ---
    private string _notificationMessage = string.Empty;
    private string _notificationColor = "#10B981";
    private bool _isNotificationVisible;

    // --- Formulaire d'Ajout ---
    private string? _newNumPatient;
    private string? _newNomPatient;
    private string? _newEtat;

    // --- Filtres de Recherche ---
    private string? _searchValeur;
    private string? _searchEtat;
    private DateTimeOffset? _searchDateHosp;
    private DateTimeOffset? _searchDateSortie;

    // --- Listes ---
    public List<string> EtatsDisponibles { get; } = new() { "Hospitalisé", "Sortie", "Non-Hospitalisé" };
    public ObservableCollection<int> ChambresDisponibles { get; } = new();
    public ObservableCollection<PatientRowItem> PatientsList { get; } = new();

    // --- Propriétés d'Ajout ---
    public string? NewNumPatient
    {
        get => _newNumPatient;
        set { _newNumPatient = value; OnPropertyChanged(); }
    }

    public string? NewNomPatient
    {
        get => _newNomPatient;
        set { _newNomPatient = value; OnPropertyChanged(); }
    }

    public string? NewEtat
    {
        get => _newEtat;
        set { _newEtat = value; OnPropertyChanged(); }
    }

    // --- Propriétés de Recherche ---
    public string? SearchValeur
    {
        get => _searchValeur;
        set { _searchValeur = value; OnPropertyChanged(); }
    }

    public string? SearchEtat
    {
        get => _searchEtat;
        set { _searchEtat = value; OnPropertyChanged(); }
    }

    public DateTimeOffset? SearchDateHosp
    {
        get => _searchDateHosp;
        set { _searchDateHosp = value; OnPropertyChanged(); }
    }

    public DateTimeOffset? SearchDateSortie
    {
        get => _searchDateSortie;
        set { _searchDateSortie = value; OnPropertyChanged(); }
    }

    // --- Notification Properties ---
    public string NotificationMessage
    {
        get => _notificationMessage;
        set { _notificationMessage = value; OnPropertyChanged(); }
    }

    public string NotificationColor
    {
        get => _notificationColor;
        set { _notificationColor = value; OnPropertyChanged(); }
    }

    public bool IsNotificationVisible
    {
        get => _isNotificationVisible;
        set { _isNotificationVisible = value; OnPropertyChanged(); }
    }

    // --- Commandes ---
    public ICommand AjouterCommand { get; }
    public ICommand ViderAjoutCommand { get; }
    public ICommand ActiverEditionLigneCommand { get; }
    public ICommand EnregistrerLigneCommand { get; }
    public ICommand AnnulerLigneCommand { get; }
    public ICommand SupprimerLigneCommand { get; }
    public ICommand RechercherCommand { get; }
    public ICommand ReinitialiserCommand { get; }

    public PatientsViewModel(IPatientServices patientServices, IChambreServices chambreServices)
    {
        _patientServices = patientServices;
        _chambreServices = chambreServices;

        AjouterCommand = new RelayCommand(async () => await AjouterPatientAsync());
        ViderAjoutCommand = new RelayCommand(ViderFormulaireAjout);

        // Passer la ligne en mode édition
        ActiverEditionLigneCommand = new RelayCommand((param) =>
        {
            if (param is PatientRowItem row)
                row.ActiverEdition();
        });

        // Enregistrer les modifications en base
        EnregistrerLigneCommand = new RelayCommand(async (param) =>
        {
            if (param is PatientRowItem row)
                await EnregistrerPatientLigneAsync(row);
        });

        // Annuler l'édition et restaurer les valeurs initiales
        AnnulerLigneCommand = new RelayCommand((param) =>
        {
            if (param is PatientRowItem row)
                row.AnnulerEdition();
        });

        SupprimerLigneCommand = new RelayCommand(async (param) =>
        {
            if (param is PatientRowItem row)
                await SupprimerPatientLigneAsync(row);
        });

        RechercherCommand = new RelayCommand(async () => await RechercherAsync());
        ReinitialiserCommand = new RelayCommand(async () => await ReinitialiserAsync());

        _ = ChargerInitialisationAsync();
    }

    private async Task ChargerInitialisationAsync()
    {
        await ChargerChambresAsync();
        await ChargerPatientsAsync();
    }

    private async Task ChargerChambresAsync()
    {
        var chambres = await _chambreServices.ListeChambre();
        ChambresDisponibles.Clear();
        foreach (var c in chambres)
        {
            if (c?.NumChambre.HasValue == true)
            {
                ChambresDisponibles.Add(c.NumChambre.Value);
            }
        }
    }

    public void AfficherNotification(bool success, string message)
    {
        NotificationMessage = message;
        NotificationColor = success ? "#10B981" : "#EF4444";
        IsNotificationVisible = true;
    }

    public async Task ChargerPatientsAsync()
    {
        var liste = await _patientServices.ListePatients();
        MettreAJourListe(liste);
    }

    private async Task AjouterPatientAsync()
    {
        var (success, message) = await _patientServices.InsererPatient(NewNumPatient, NewNomPatient, NewEtat);
        AfficherNotification(success, message);

        if (success)
        {
            ViderFormulaireAjout();
            await ChargerPatientsAsync();
        }
    }

    private async Task EnregistrerPatientLigneAsync(PatientRowItem? row)
    {
        if (row == null || string.IsNullOrWhiteSpace(row.NumPatient))
        {
            AfficherNotification(false, "Patient invalide.");
            return;
        }

        DateOnly? dateHosp = row.DateHosp.HasValue
            ? DateOnly.FromDateTime(row.DateHosp.Value.DateTime)
            : null;

        DateOnly? dateSortie = row.DateSortie.HasValue
            ? DateOnly.FromDateTime(row.DateSortie.Value.DateTime)
            : null;

        var (success, message) = await _patientServices.UpdatePatient(
            row.NumPatient,
            row.NomPatient,
            row.Etat ?? "",
            dateHosp,
            dateSortie,
            row.NumChambre
        );

        AfficherNotification(success, message);

        if (success)
        {
            row.TerminerEdition();
            await ChargerPatientsAsync();
        }
    }

    private async Task SupprimerPatientLigneAsync(PatientRowItem? row)
    {
        if (row == null || string.IsNullOrWhiteSpace(row.NumPatient))
        {
            AfficherNotification(false, "Patient invalide.");
            return;
        }

        var (success, message) = await _patientServices.DeletePatient(row.NumPatient);
        AfficherNotification(success, message);

        if (success)
        {
            await ChargerPatientsAsync();
        }
    }

    private async Task RechercherAsync()
    {
        DateOnly? dateHosp = SearchDateHosp.HasValue ? DateOnly.FromDateTime(SearchDateHosp.Value.DateTime) : null;
        DateOnly? dateSortie = SearchDateSortie.HasValue ? DateOnly.FromDateTime(SearchDateSortie.Value.DateTime) : null;

        var liste = await _patientServices.RecherchePatient(
            string.IsNullOrWhiteSpace(SearchValeur) ? null : SearchValeur,
            string.IsNullOrWhiteSpace(SearchEtat) ? null : SearchEtat,
            dateHosp,
            dateSortie
        );

        MettreAJourListe(liste);
        IsNotificationVisible = false;
    }

    private async Task ReinitialiserAsync()
    {
        SearchValeur = null;
        SearchEtat = null;
        SearchDateHosp = null;
        SearchDateSortie = null;
        IsNotificationVisible = false;
        await ChargerPatientsAsync();
    }

    public void ViderFormulaireAjout()
    {
        NewNumPatient = string.Empty;
        NewNomPatient = string.Empty;
        NewEtat = null;
    }

    private void MettreAJourListe(List<Patient?> liste)
    {
        PatientsList.Clear();
        foreach (var p in liste)
        {
            if (p != null)
            {
                PatientsList.Add(new PatientRowItem(p));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// --- CLASSE REPRÉSENTANT UNE LIGNE ÉDITABLE DANS LE TABLEAU ---
public class PatientRowItem : INotifyPropertyChanged
{
    private string _numPatient = string.Empty;
    private string _nomPatient = string.Empty;
    private string _etat = string.Empty;
    private DateTimeOffset? _dateHosp;
    private DateTimeOffset? _dateSortie;
    private int? _numChambre;
    private bool _isEditing;

    // Champs de sauvegarde pour l'annulation
    private string _backupNomPatient = string.Empty;
    private string _backupEtat = string.Empty;
    private DateTimeOffset? _backupDateHosp;
    private DateTimeOffset? _backupDateSortie;
    private int? _backupNumChambre;

    public string NumPatient
    {
        get => _numPatient;
        set { _numPatient = value; OnPropertyChanged(); }
    }

    public string NomPatient
    {
        get => _nomPatient;
        set { _nomPatient = value; OnPropertyChanged(); }
    }

    public string Etat
    {
        get => _etat;
        set { _etat = value; OnPropertyChanged(); }
    }

    public DateTimeOffset? DateHosp
    {
        get => _dateHosp;
        set { _dateHosp = value; OnPropertyChanged(); }
    }

    public DateTimeOffset? DateSortie
    {
        get => _dateSortie;
        set { _dateSortie = value; OnPropertyChanged(); }
    }

    public int? NumChambre
    {
        get => _numChambre;
        set { _numChambre = value; OnPropertyChanged(); }
    }

    public bool IsEditing
    {
        get => _isEditing;
        set { _isEditing = value; OnPropertyChanged(); }
    }

    public PatientRowItem(Patient p)
    {
        _numPatient = p.NumPatient ?? string.Empty;
        _nomPatient = p.NomPatient ?? string.Empty;
        _etat = p.Etat ?? string.Empty;
        _dateHosp = p.DateHosp.HasValue ? new DateTimeOffset(p.DateHosp.Value.ToDateTime(TimeOnly.MinValue)) : null;
        _dateSortie = p.DateSortie.HasValue ? new DateTimeOffset(p.DateSortie.Value.ToDateTime(TimeOnly.MinValue)) : null;
        _numChambre = p.NumChambre;
    }

    public void ActiverEdition()
    {
        _backupNomPatient = NomPatient;
        _backupEtat = Etat;
        _backupDateHosp = DateHosp;
        _backupDateSortie = DateSortie;
        _backupNumChambre = NumChambre;
        IsEditing = true;
    }

    public void AnnulerEdition()
    {
        NomPatient = _backupNomPatient;
        Etat = _backupEtat;
        DateHosp = _backupDateHosp;
        DateSortie = _backupDateSortie;
        NumChambre = _backupNumChambre;
        IsEditing = false;
    }

    public void TerminerEdition()
    {
        IsEditing = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}