namespace GestionHospitaliere.Backend.Interfaces;

using GestionHospitaliere.Backend.Models;

public interface IPatientRepository
{
    //Recuperer tous les patients
    Task<IEnumerable<Patient?>> GetAllAsync();
    //Recuperer patient par son numero numpatient
    Task<Patient?> GetByIdAsync(string numpatient);
    //Recuperer patient par etat
    Task<IEnumerable<Patient?>> GetByEtatAsync(string etat);
    //Recuperer patient par like(sans datehosp et datesortie)
    Task<IEnumerable<Patient?>> GetByLikeAsync(string valeur);
    //Recuperer patient par datehosp
    Task<IEnumerable<Patient?>> GetByDateHospAsync(DateOnly date);
    //Recuperer patient par datesortie
    Task<IEnumerable<Patient?>> GetByDateSortieAsync(DateOnly date);
    //recuperer par datehosp et datesortie
    Task<IEnumerable<Patient?>> GetByDatehospDatesortieAsync(DateOnly datehosp,DateOnly datesortie);
    //recuperer par like et par etat
    Task<IEnumerable<Patient?>> GetByLikeEtatAsync(string valeur, string etat);
    //recuperer par tout
    Task<IEnumerable<Patient?>> GetByAllAsync(string valeur,string etat, DateOnly datehosp, DateOnly datesortie);
    
    //Ajouter un patient
    Task<bool> AddAsync(Patient patient);
    //Mis à jour
    Task<bool> UpdateAsync(Patient patient);
    //Supprimer un patient
    Task<bool> DeleteAsync(string numpatient);
}
