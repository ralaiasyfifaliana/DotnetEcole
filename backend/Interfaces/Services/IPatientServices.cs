using GestionHospitaliere.Backend.Models;

namespace GestionHospitaliere.Backend.Interfaces.Services;

public interface IPatientServices
{
    //Inserer un patient
    Task<(bool Success,string Message)> InsererPatient(string? numpatient,string? nompatient,string? etat);
    //Rechercher un patient
    Task<List<Patient?>> RecherchePatient(string? valeur,string? etat,DateOnly? datehosp,DateOnly? datesortie );
    //Supprimer un patient
    Task<(bool Success, string Message)> DeletePatient(string numpatient);
    //modifier un patient
    Task<(bool Success, string Message)> UpdatePatient(string numpatient, string? nompatient, string etat, DateOnly? datehosp,
        DateOnly? datesortie, int? numchambre);
    //liste patients
    Task<List<Patient?>> ListePatients();
}
