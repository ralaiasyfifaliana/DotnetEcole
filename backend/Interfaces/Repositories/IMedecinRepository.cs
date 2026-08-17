namespace GestionHospitaliere.Backend.Interfaces;

using GestionHospitaliere.Backend.Models;

public interface IMedecinRepository
{
    //Recuperer tout les medecin
    Task<IEnumerable<Medecin?>> GetAllAsync();
    //Recuperation du medecin par son codemd
    Task<Medecin?> GetByIdAsync(string?  codemed);
    //Recuperation medecin par sa poste
    Task<IEnumerable<Medecin?>> GetPosAsync(string poste);
    //Recuperation medecin par like
    Task<IEnumerable<Medecin?>> GetLikeAsync(string valeur);
    //Recuperation medecin par like et poste
    Task<IEnumerable<Medecin?>> GetLikePosteAsync(string valeur,string poste);
    //Ajout medecin
    Task<bool> AddAsync(Medecin medecin);
    //Mis à jour
    Task<bool> UpdateAsync(Medecin medecin);
    //Suppression
    Task<bool> DeleteAsync(string codemed);
}
