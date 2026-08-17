namespace GestionHospitaliere.Backend.Interfaces.Services;

using GestionHospitaliere.Backend.Models;

public interface IMedecinServices
{
    //Recuperer tout les medecin: pour liste déroulante
    Task<List<Medecin?>> ListeMedecin();
    //Recuperer un medecin par son codemed
    Task<Medecin?> GetMedecinAsync(string? codemed);
    //Inserer un medecin dans la base de donné
    Task<(bool Success, string Message)> InsererMedecin(string? codemed, string? nommed, string? poste,string? password,string? type);
    //Rechercher Medecins
    Task<List<Medecin?>> RechercheMedecin(string? value,string? poste);
    //Supprimer un medecin
    Task<(bool Success,string Message)> SupprimerMedecin(string codemed);
    //Mettre à jour un medecin
    Task<(bool Success,string Message)> MettreJourMedecin(string codemed,string? nommed,string? poste);
}