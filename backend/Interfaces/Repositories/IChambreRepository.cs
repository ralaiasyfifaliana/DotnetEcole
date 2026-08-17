namespace GestionHospitaliere.Backend.Interfaces;

using GestionHospitaliere.Backend.Models;

public interface IChambreRepository
{
    //Recuperer les chambres avec les patients(listage chambre)
    Task<IEnumerable<Chambre?>> GetAllAsync();
    //Recherche Chambre par son numero
    Task<Chambre?> GetByIdAsync(int? numchambre);
}
