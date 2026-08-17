namespace GestionHospitaliere.Frontend.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Models;

public class ChambresViewModel : INotifyPropertyChanged
{
    private readonly IChambreServices _chambreServices;

    private string _notificationMessage = string.Empty;
    private string _notificationColor = "#10B981";
    private bool _isNotificationVisible;

    private string? _searchNumChambre;

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

    public string? SearchNumChambre
    {
        get => _searchNumChambre;
        set { _searchNumChambre = value; OnPropertyChanged(); }
    }

    public ObservableCollection<ChambreItemViewModel> ChambresList { get; } = new();

    public ICommand RechercherCommand { get; }
    public ICommand ReinitialiserCommand { get; }

    public ChambresViewModel(IChambreServices chambreServices)
    {
        _chambreServices = chambreServices;

        RechercherCommand = new RelayCommand(async () => await RechercherAsync());
        ReinitialiserCommand = new RelayCommand(async () => await ReinitialiserAsync());

        _ = ChargerChambresAsync();
    }

    public void AfficherNotification(bool success, string message)
    {
        NotificationMessage = message;
        NotificationColor = success ? "#10B981" : "#EF4444";
        IsNotificationVisible = true;
    }

    public async Task ChargerChambresAsync()
    {
        var liste = await _chambreServices.ListeChambre();
        MettreAJourListe(liste);
    }

    private async Task RechercherAsync()
    {
        if (int.TryParse(SearchNumChambre, out int numChambre))
        {
            var chambre = await _chambreServices.RechercheChambre(numChambre);
            var liste = chambre != null ? new List<Chambre?> { chambre } : new List<Chambre?>();
            MettreAJourListe(liste);

            if (chambre == null)
            {
                AfficherNotification(false, $"Aucune chambre trouvée avec le numéro {numChambre}.");
            }
            else
            {
                IsNotificationVisible = false;
            }
        }
        else if (string.IsNullOrWhiteSpace(SearchNumChambre))
        {
            IsNotificationVisible = false;
            await ChargerChambresAsync();
        }
        else
        {
            AfficherNotification(false, "Veuillez entrer un numéro de chambre valide.");
        }
    }

    private async Task ReinitialiserAsync()
    {
        SearchNumChambre = string.Empty;
        IsNotificationVisible = false;
        await ChargerChambresAsync();
    }

    private void MettreAJourListe(List<Chambre?> liste)
    {
        ChambresList.Clear();
        foreach (var chambre in liste)
        {
            if (chambre != null)
            {
                ChambresList.Add(new ChambreItemViewModel(chambre));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Wrapper pour l'affichage de chaque ligne dans le DataGrid
public class ChambreItemViewModel : INotifyPropertyChanged
{
    public Chambre Model { get; }

    public int? NumChambre => Model.NumChambre;
    public int? NbrLit => Model.Nbr_lit;

    // Calcul du nombre de patients présents
    public int LitsOccupes => Model.Patients?.Count ?? 0;

    // Statut basé sur la capacité max Nbr_lit
    public string StatutChambre => (NbrLit.HasValue && LitsOccupes >= NbrLit.Value) ? "Occupée" : "Disponible";

    public string StatutColor => (NbrLit.HasValue && LitsOccupes >= NbrLit.Value) ? "#EF4444" : "#10B981";

    // Formatage des patients occupant la chambre
    public string SummaryPatients
    {
        get
        {
            if (Model.Patients == null || !Model.Patients.Any())
                return "Aucun patient";

            var liste = Model.Patients
                .Where(p => p != null)
                .Select(p => $"{p!.NumPatient ?? "N/A"} - {p.NomPatient ?? "Nom inconnu"}");

            return string.Join(", ", liste);
        }
    }

    public ChambreItemViewModel(Chambre model)
    {
        Model = model;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
