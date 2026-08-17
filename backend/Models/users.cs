namespace GestionHospitaliere.Backend.Models;

public class Users
{
    public int? IdUser{get;}
    public string? Login{get;set;}
    public string? Password{get;set;}
    public string? Type{get;set;}
    public Users(int? iduser,string? login,string? password,string? type)
    {
        IdUser = iduser;
        Login = login;
        Password = password;
        Type = type;
    }
}