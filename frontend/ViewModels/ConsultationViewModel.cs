namespace GestionHospitaliere.Frontend.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Data.Converters;
using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Models;

public class ConsultationsViewModel : INotifyPropertyChanged
{
    private readonly IConsultationServices _consultationService;
    private readonly IMedecinServices? _medecinService;
    private readonly IPatientServices? _patientService;

    // --- NOTIFICATION ---
    private string _notificationMessage = string.Empty;
    private string _notificationColor = "#10B981";
    private bool _isNotificationVisible;

    // --- FORMULAIRE DE CRÉATION ---
    private string? _selectedType;
    private string? _newFrais;
    private DateTimeOffset? _newDate = DateTimeOffset.Now;
    private TimeSpan? _newTime = DateTime.Now.TimeOfDay;
    private Medecin? _selectedMedecin;
    private Patient? _selectedPatient;

    // --- RECHERCHE ---
    private string? _searchType;
    private DateTimeOffset? _searchDate1;
    private DateTimeOffset? _searchDate2;
    private Medecin? _searchSelectedMedecin;
    private Patient? _searchSelectedPatient;

    // --- ÉDITION INLINE ---
    private Consultation? _editingConsultation;
    private string? _editType;
    private string? _editFrais;
    private Medecin? _editSelectedMedecin;
    private Patient? _editSelectedPatient;
    private DateTimeOffset? _editDate;
    private TimeSpan? _editTime;
    private string? _editEtatFacture;

    // --- COLLECTIONS ---
    public ObservableCollection<Consultation> ConsultationsList { get; } = new();
    public ObservableCollection<string> TypesList { get; } = new()
    {
        "Générale", "Pédiatrie", "Dentaire", "Urgences"
    };
    public ObservableCollection<Medecin> MedecinsList { get; } = new();
    public ObservableCollection<Patient> PatientsList { get; } = new();
    public List<string> EtatsFactureList { get; } = new() { "Non-Payé", "Payé" };

    // --- CONVERTISSEURS ---
    public static readonly IMultiValueConverter EqualsConverter =
        new FuncMultiValueConverter<object?, bool>(values =>
        {
            var v = values.ToList();
            return v.Count >= 2 && Equals(v[0], v[1]);
        });

    public static readonly IMultiValueConverter NotEqualsConverter =
        new FuncMultiValueConverter<object?, bool>(values =>
        {
            var v = values.ToList();
            return v.Count < 2 || !Equals(v[0], v[1]);
        });

    public static readonly IMultiValueConverter MedecinCodeConverter =
        new FuncMultiValueConverter<object?, string>(values =>
        {
            var v = values.ToList();
            if (v.Count < 2) return "-";
            var idMed = v[0] as int?;
            var medecins = v[1] as IEnumerable<Medecin>;
            if (idMed == null || medecins == null) return "-";
            return medecins.FirstOrDefault(m => m?.Idmed == idMed)?.CodeMed ?? "-";
        });

    public static readonly IMultiValueConverter PatientNumConverter =
        new FuncMultiValueConverter<object?, string>(values =>
        {
            var v = values.ToList();
            if (v.Count < 2) return "-";
            var idPatient = v[0] as int?;
            var patients = v[1] as IEnumerable<Patient>;
            if (idPatient == null || patients == null) return "-";
            return patients.FirstOrDefault(p => p?.IdPatient == idPatient)?.NumPatient ?? "-";
        });

    // --- PROPRIÉTÉS NOTIFICATION ---
    public string NotificationMessage { get => _notificationMessage; set => SetProperty(ref _notificationMessage, value); }
    public string NotificationColor { get => _notificationColor; set => SetProperty(ref _notificationColor, value); }
    public bool IsNotificationVisible { get => _isNotificationVisible; set => SetProperty(ref _isNotificationVisible, value); }

    // --- PROPRIÉTÉS CRÉATION ---
    public string? SelectedType { get => _selectedType; set => SetProperty(ref _selectedType, value); }
    public string? NewFrais { get => _newFrais; set => SetProperty(ref _newFrais, value); }
    public DateTimeOffset? NewDate { get => _newDate; set => SetProperty(ref _newDate, value); }
    public TimeSpan? NewTime { get => _newTime; set => SetProperty(ref _newTime, value); }
    public Medecin? SelectedMedecin { get => _selectedMedecin; set => SetProperty(ref _selectedMedecin, value); }
    public Patient? SelectedPatient { get => _selectedPatient; set => SetProperty(ref _selectedPatient, value); }

    // --- PROPRIÉTÉS RECHERCHE ---
    public string? SearchType { get => _searchType; set => SetProperty(ref _searchType, value); }
    public DateTimeOffset? SearchDate1 { get => _searchDate1; set => SetProperty(ref _searchDate1, value); }
    public DateTimeOffset? SearchDate2 { get => _searchDate2; set => SetProperty(ref _searchDate2, value); }
    public Medecin? SearchSelectedMedecin { get => _searchSelectedMedecin; set => SetProperty(ref _searchSelectedMedecin, value); }
    public Patient? SearchSelectedPatient { get => _searchSelectedPatient; set => SetProperty(ref _searchSelectedPatient, value); }

    // --- PROPRIÉTÉS ÉDITION ---
    public Consultation? EditingConsultation { get => _editingConsultation; private set => SetProperty(ref _editingConsultation, value); }
    public string? EditType { get => _editType; set => SetProperty(ref _editType, value); }
    public string? EditFrais { get => _editFrais; set => SetProperty(ref _editFrais, value); }
    public Medecin? EditSelectedMedecin { get => _editSelectedMedecin; set => SetProperty(ref _editSelectedMedecin, value); }
    public Patient? EditSelectedPatient { get => _editSelectedPatient; set => SetProperty(ref _editSelectedPatient, value); }
    public DateTimeOffset? EditDate { get => _editDate; set => SetProperty(ref _editDate, value); }
    public TimeSpan? EditTime { get => _editTime; set => SetProperty(ref _editTime, value); }
    public string? EditEtatFacture { get => _editEtatFacture; set => SetProperty(ref _editEtatFacture, value); }

    // --- COMMANDES ---
    public ICommand ViderFormulaireCommand { get; }
    public ICommand PlanifierCommand { get; }
    public ICommand RechercherCommand { get; }
    public ICommand ReinitialiserRechercheCommand { get; }
    public ICommand CommencerEditionCommand { get; }
    public ICommand SauvegarderEditionCommand { get; }
    public ICommand AnnulerEditionCommand { get; }
    public ICommand SupprimerConsultationCommand { get; }

    public ConsultationsViewModel(
        IConsultationServices consultationService,
        IMedecinServices? medecinService = null,
        IPatientServices? patientService = null)
    {
        _consultationService = consultationService;
        _medecinService = medecinService;
        _patientService = patientService;

        ViderFormulaireCommand = new RelayCommand(ViderFormulaire);
        PlanifierCommand = new RelayCommand(async () => await PlanifierAsync());
        RechercherCommand = new RelayCommand(async () => await RechercherAsync());
        ReinitialiserRechercheCommand = new RelayCommand(async () => await ReinitialiserRechercheAsync());

        CommencerEditionCommand = new RelayCommand<Consultation>(CommencerEdition);
        SauvegarderEditionCommand = new RelayCommand(async () => await SauvegarderEditionAsync());
        AnnulerEditionCommand = new RelayCommand(() => EditingConsultation = null);
        SupprimerConsultationCommand = new RelayCommand<Consultation>(async c => await SupprimerConsultationAsync(c));

        _ = InitialiserDonneesAsync();
    }

    private async Task InitialiserDonneesAsync()
    {
        await ChargerReferentielsAsync();
        await ChargerConsultationsAsync();
    }

    private async Task ChargerReferentielsAsync()
    {
        if (_medecinService != null)
            RemplirCollection(MedecinsList, await _medecinService.ListeMedecin());

        if (_patientService != null)
            RemplirCollection(PatientsList, await _patientService.ListePatients());
    }

    public async Task ChargerConsultationsAsync()
    {
        var liste = await _consultationService.ListeConsultation();
        RemplirCollection(ConsultationsList, liste);
    }

    private async Task PlanifierAsync()
    {
        int.TryParse(NewFrais, out int fraisValue);

        var (success, message) = await _consultationService.PlanifierConsultation(
            SelectedType,
            CombinerDateEtHeure(NewDate, NewTime),
            fraisValue,
            GetCodeMedecin(SelectedMedecin),
            GetNumPatient(SelectedPatient)
        );

        AfficherNotification(message, success);

        if (success)
        {
            ViderFormulaire();
            await ChargerConsultationsAsync();
        }
    }

    private void CommencerEdition(Consultation? c)
    {
        if (c == null) return;

        EditingConsultation = c;
        EditType = c.Type;
        EditFrais = c.Frais.ToString();
        EditSelectedMedecin = MedecinsList.FirstOrDefault(m => m.Idmed == c.IdMed);
        EditSelectedPatient = PatientsList.FirstOrDefault(p => p.IdPatient == c.IdPatient);
        EditEtatFacture = c.EtatFacture ?? "Non-Payé";
        EditDate = c.Date.HasValue ? new DateTimeOffset(c.Date.Value) : null;
        EditTime = c.Date?.TimeOfDay;
    }

    private async Task SauvegarderEditionAsync()
    {
        if (EditingConsultation == null) return;

        int.TryParse(EditFrais, out int fraisValue);

        var (success, message) = await _consultationService.ModifierConsultation(
            EditingConsultation.RefCons,
            EditType,
            CombinerDateEtHeure(EditDate, EditTime),
            fraisValue,
            GetCodeMedecin(EditSelectedMedecin),
            GetNumPatient(EditSelectedPatient),
            EditingConsultation.Prescription,
            EditingConsultation.ObjetFacture,
            EditEtatFacture
        );

        AfficherNotification(message, success);

        if (success)
        {
            EditingConsultation = null;
            await ChargerConsultationsAsync();
        }
    }

    private async Task SupprimerConsultationAsync(Consultation? consultation)
    {
        if (consultation?.RefCons == null) return;

        var (success, message) = await _consultationService.AnnulerConsultation(consultation.RefCons.Value);
        AfficherNotification(message, success);

        if (success) ConsultationsList.Remove(consultation);
    }

    private async Task RechercherAsync()
    {
        var resultats = await _consultationService.RechercheConsultation(
            string.IsNullOrWhiteSpace(SearchType) ? null : SearchType,
            SearchDate1.HasValue ? DateOnly.FromDateTime(SearchDate1.Value.Date) : null,
            SearchDate2.HasValue ? DateOnly.FromDateTime(SearchDate2.Value.Date) : null,
            GetCodeMedecin(SearchSelectedMedecin),
            GetNumPatient(SearchSelectedPatient)
        );

        RemplirCollection(ConsultationsList, resultats);
    }

    private async Task ReinitialiserRechercheAsync()
    {
        SearchType = null;
        SearchDate1 = null;
        SearchDate2 = null;
        SearchSelectedMedecin = null;
        SearchSelectedPatient = null;

        await ChargerConsultationsAsync();
    }

    private void ViderFormulaire()
    {
        SelectedType = null;
        NewFrais = null;
        NewDate = DateTimeOffset.Now;
        NewTime = DateTime.Now.TimeOfDay;
        SelectedMedecin = null;
        SelectedPatient = null;
    }

    private async void AfficherNotification(string message, bool success)
    {
        NotificationMessage = message;
        NotificationColor = success ? "#10B981" : "#EF4444";
        IsNotificationVisible = true;

        await Task.Delay(4000);
        IsNotificationVisible = false;
    }

    private static void RemplirCollection<T>(ObservableCollection<T> collection, IEnumerable<T?> items)
    {
        collection.Clear();
        foreach (var item in items)
        {
            if (item != null) collection.Add(item);
        }
    }

    private static string? GetCodeMedecin(Medecin? m) => m?.CodeMed ?? m?.Idmed.ToString();
    private static string? GetNumPatient(Patient? p) => p?.NumPatient ?? p?.IdPatient.ToString();

    private static DateTime? CombinerDateEtHeure(DateTimeOffset? date, TimeSpan? time)
    {
        if (!date.HasValue) return null;
        return date.Value.Date.Add(time ?? TimeSpan.Zero);
    }

    // --- INotifyPropertyChanged ---
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(storage, value)) return false;
        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}