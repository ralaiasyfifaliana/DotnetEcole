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

public class MedecinsViewModel : INotifyPropertyChanged
{
    private readonly IMedecinServices _medecinServices;

    // --- Champs du Formulaire d'Ajout ---
    private string? _codeMed;
    private string? _nomMed;
    private string? _selectedPoste;
    private string? _password;

    // --- Notification ---
    private string _notificationMessage = string.Empty;
    private string _notificationColor = "#10B981";
    private bool _isNotificationVisible;

    // --- Champs de Recherche ---
    private string? _searchQuery;
    private string? _searchPoste;

    // --- Propriétés Publiques ---
    public string? CodeMed
    {
        get => _codeMed;
        set { _codeMed = value; OnPropertyChanged(); }
    }

    public string? NomMed
    {
        get => _nomMed;
        set { _nomMed = value; OnPropertyChanged(); }
    }

    public string? SelectedPoste
    {
        get => _selectedPoste;
        set { _selectedPoste = value; OnPropertyChanged(); }
    }

    public string? Password
    {
        get => _password;
        set { _password = value; OnPropertyChanged(); }
    }

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

    public string? SearchQuery
    {
        get => _searchQuery;
        set { _searchQuery = value; OnPropertyChanged(); }
    }

    public string? SearchPoste
    {
        get => _searchPoste;
        set { _searchPoste = value; OnPropertyChanged(); }
    }

    public List<string> PostesList { get; } = new()
    {
        "Généraliste",
        "Dentiste",
        "Ophtalmologue",
        "Hématologue"
    };

    public ObservableCollection<MedecinItemViewModel> MedecinsList { get; } = new();

    // Commandes Principales
    public ICommand AjouterMedecinCommand { get; }
    public ICommand RechercherCommand { get; }
    public ICommand ReinitialiserRechercheCommand { get; }

    public MedecinsViewModel(IMedecinServices medecinServices)
    {
        _medecinServices = medecinServices;

        AjouterMedecinCommand = new RelayCommand(async () => await AjouterMedecinAsync());
        RechercherCommand = new RelayCommand(async () => await RechercherAsync());
        ReinitialiserRechercheCommand = new RelayCommand(async () => await ReinitialiserRechercheAsync());

        _ = ChargerMedecinsAsync();
    }

    public void AfficherNotification(bool success, string message)
    {
        NotificationMessage = message;
        NotificationColor = success ? "#10B981" : "#EF4444";
        IsNotificationVisible = true;
    }

    public async Task ChargerMedecinsAsync()
    {
        var liste = await _medecinServices.ListeMedecin();
        MettreAJourListe(liste);
    }

    private async Task AjouterMedecinAsync()
    {
        var (success, message) = await _medecinServices.InsererMedecin(
            CodeMed, NomMed, SelectedPoste, Password, "medecin");

        AfficherNotification(success, message);

        if (success)
        {
            CodeMed = string.Empty;
            NomMed = string.Empty;
            SelectedPoste = null;
            Password = string.Empty;

            await ChargerMedecinsAsync();
        }
    }

    private async Task RechercherAsync()
    {
        var liste = await _medecinServices.RechercheMedecin(SearchQuery, SearchPoste);
        MettreAJourListe(liste);
    }

    private async Task ReinitialiserRechercheAsync()
    {
        SearchQuery = string.Empty;
        SearchPoste = null;
        await ChargerMedecinsAsync();
    }

    public async Task SupprimerMedecinAsync(MedecinItemViewModel item)
    {
        if (item.Model.CodeMed == null) return;

        var (success, message) = await _medecinServices.SupprimerMedecin(item.Model.CodeMed);
        AfficherNotification(success, message);

        if (success)
        {
            await ChargerMedecinsAsync();
        }
    }

    public async Task ValiderModificationAsync(MedecinItemViewModel item)
    {
        if (item.Model.CodeMed == null) return;

        var (success, message) = await _medecinServices.MettreJourMedecin(
            item.Model.CodeMed, item.EditNomMed, item.EditPoste);

        AfficherNotification(success, message);

        if (success)
        {
            item.IsEditing = false;
            await ChargerMedecinsAsync();
        }
    }

    private void MettreAJourListe(List<Medecin?> liste)
    {
        MedecinsList.Clear();
        foreach (var medecin in liste)
        {
            if (medecin != null)
            {
                MedecinsList.Add(new MedecinItemViewModel(medecin, this, PostesList));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// --- Classe Wrapper pour chaque ligne de médecin ---
public class MedecinItemViewModel : INotifyPropertyChanged
{
    private readonly MedecinsViewModel _parentViewModel;
    private bool _isEditing;
    private string? _editNomMed;
    private string? _editPoste;

    public Medecin Model { get; }
    public List<string> PostesList { get; }

    public bool IsEditing
    {
        get => _isEditing;
        set
        {
            _isEditing = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotEditing));
            OnPropertyChanged(nameof(ButtonModifierText));
            OnPropertyChanged(nameof(ButtonModifierColor));
        }
    }

    public bool IsNotEditing => !IsEditing;

    public string ButtonModifierText => IsEditing ? "Valider" : "Modifier";
    public string ButtonModifierColor => IsEditing ? "#10B981" : "#F59E0B";

    public string? EditNomMed
    {
        get => _editNomMed;
        set { _editNomMed = value; OnPropertyChanged(); }
    }

    public string? EditPoste
    {
        get => _editPoste;
        set { _editPoste = value; OnPropertyChanged(); }
    }

    public ICommand BasculerOuValiderEditionCommand { get; }
    public ICommand AnnulerEditionCommand { get; }
    public ICommand SupprimerCommand { get; }

    public MedecinItemViewModel(Medecin model, MedecinsViewModel parentViewModel, List<string> postesList)
    {
        Model = model;
        _parentViewModel = parentViewModel;
        PostesList = postesList;

        EditNomMed = model.NomMed;
        EditPoste = model.Poste;

        BasculerOuValiderEditionCommand = new RelayCommand(async () =>
        {
            if (IsEditing)
            {
                await _parentViewModel.ValiderModificationAsync(this);
            }
            else
            {
                IsEditing = true;
            }
        });

        AnnulerEditionCommand = new RelayCommand(() =>
        {
            EditNomMed = Model.NomMed;
            EditPoste = Model.Poste;
            IsEditing = false;
        });

        SupprimerCommand = new RelayCommand(async () =>
        {
            await _parentViewModel.SupprimerMedecinAsync(this);
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
