namespace GestionHospitaliere.Backend.Interfaces;

using GestionHospitaliere.Backend.Models;

public interface IUsersRepository
{
    //Recuperer par login
    Task<Users?> GetUsersAsync(string login);
    //Creer utilistaeurs
    Task<bool> AddAsync(Users user);

    //Suppression
    Task<bool> DeleteAsync(string login);
}