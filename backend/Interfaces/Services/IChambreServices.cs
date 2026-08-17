using GestionHospitaliere.Backend.Models;

namespace GestionHospitaliere.Backend.Interfaces.Services;

public interface IChambreServices
{
    //Recuperer tout les chambre: liste déroulante
    Task<List<Chambre?>> ListeChambre();
    //Rechercher une chambre: numero
    Task<Chambre?> RechercheChambre(int  numchambre);
}