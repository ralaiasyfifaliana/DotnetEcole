namespace GestionHospitaliere.Backend.Interfaces;

using GestionHospitaliere.Backend.Models;

public interface IConsultationRepository
{
    //Recupère toutes les consultations
    Task<IEnumerable<Consultation?>> GetAllAsync();
    //Recuperation du consulatation par son reference
    Task<Consultation?> GetByIdAsync(int refcons);
    //Recupere les consultation par type
    Task<IEnumerable<Consultation?>> GetByTypeAsync(string type);
    //Recuperer les consultation par medecin
    Task<IEnumerable<Consultation?>> GetByMedAsync(int idmed);
    //Recuperer les consultation par patient
    Task<IEnumerable<Consultation?>> GetByPatAsync(int idpatient);
    //Recuperer les consultations par date
    Task<IEnumerable<Consultation?>>GetByDateAsync(DateOnly? date);
    //Recuperer les consultation par medecin et par date
    Task<IEnumerable<Consultation?>> GetByMedDateAsync(int idmed, DateOnly? date);
    //Recuperer les consultation par type et par date
    Task<IEnumerable<Consultation?>> GetByTypeDateAsync(string type, DateOnly? date);
    //Recuperer consultation entre 2 dates
    Task<IEnumerable<Consultation?>> GetBetweenAsync(DateOnly? date1, DateOnly? date2);
    //Planifier une consultation
    Task<bool> AddAsync(Consultation consultation);
    //Modifier une consultation
    Task<bool> UpdateAsync(Consultation consultation);
    //Annuler une consuiltation
    Task<bool> DeleteAsync(int refcons);
    //liste consultation par medecin,par date,et objet facture est null(consultation en attente)
    Task<IEnumerable<Consultation?>> GetDefaultAsync(int idmed, DateOnly? date);
    //liste consultation par medecin,par date,et objet facture est non null(consultation achevé)
    Task<IEnumerable<Consultation?>> GetFinishAsync(int idmed, DateOnly? date);

    //liste consultation entre 2 dates et par idmed
    Task<IEnumerable<Consultation?>> GetBetweenByMedAsync(int idmed, DateOnly? date1, DateOnly? date2);
}