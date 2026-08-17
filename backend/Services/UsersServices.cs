namespace GestionHospitaliere.Backend.Services;

using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Models;
using GestionHospitaliere.Backend.Interfaces;
public class UsersServices : IUsersServices
{
    private IUsersRepository _usersrepository;
    public UsersServices(IUsersRepository usersrepository)
    {
        _usersrepository = usersrepository;
    }

    public async Task<(bool Success, string Message)> InsererUtilisateur(string? login,string? password,string? type)
    {
        try
        {
            if(password == null || password == "" || password.Trim() == "")
            {
                return (false,"Entrer un mot de passe");
            }
            if (type == null)
            {
                return (false, "Choisir le type");
            }
            if (login == null) return (false, "Login obligatoire");
            Users user = new Users(null,login.ToUpper(),password,type);
            if (await _usersrepository.AddAsync(user))
            {
                return (true,"Compte creer avec success");
            }
            return (false,"Erreur ducreation de code");
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Error: {exc}");
            return (false,$"Error: {exc}");
        }
    }
    public async Task<Users?> SeConnecter(string? login,string? password)
    {
        try
        {
            if(login == null || login == "" || login.Trim() == "")
            {
                return null;
            }
            var user = await _usersrepository.GetUsersAsync(login);
            if(user != null && password != null && user.Password == Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password))).ToLower())
            {
                return user;
            }
            return null;
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Error: {exc}");
            return null;
        }
    }

}
