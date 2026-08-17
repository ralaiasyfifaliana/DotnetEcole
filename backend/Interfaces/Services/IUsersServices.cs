namespace GestionHospitaliere.Backend.Interfaces.Services;

using GestionHospitaliere.Backend.Models;
public interface IUsersServices
{
    //Inserer une nouvelle utilistaeur
    Task<(bool Success, string Message)> InsererUtilisateur(string? login,string? password,string? type);
    //Se connecter
    Task<Users?> SeConnecter(string? login,string? password);
}