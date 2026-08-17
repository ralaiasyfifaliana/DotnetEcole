namespace GestionHospitaliere.Backend.Data;

using Microsoft.Extensions.Configuration;
using Npgsql;

public class Database
{
    private readonly string _connectionString;

    public Database(IConfiguration configuration)
    {
        // Récupère la chaîne "DefaultConnection" depuis appsettings.json
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("La chaîne de connexion 'DefaultConnection' est introuvable.");
    }

    // Fonction pour se connecter au base de données
    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }

    // Méthode de test avec try / catch et if
    public async Task<bool> TesterConnexionAsync()
    {
        try
        {
            await using var connection = CreateConnection();
            await connection.OpenAsync();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (NpgsqlException ex)
        {
            Console.WriteLine($"[Erreur PostgreSQL] : {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Erreur Générale] : {ex.Message}");
            return false;
        }
    }
}
