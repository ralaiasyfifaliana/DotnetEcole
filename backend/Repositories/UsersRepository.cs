namespace GestionHospitaliere.Backend.Repositories;

using GestionHospitaliere.Backend.Data;
using GestionHospitaliere.Backend.Interfaces;
using GestionHospitaliere.Backend.Models;
using Npgsql;

public class UsersRepository : IUsersRepository
{
    private readonly Database _database;

    // implémente la connexion à la base de données
    public UsersRepository(Database database)
    {
        _database = database;
    }
    //recuperer iduser
    public async Task<Users?> GetUsersAsync(string login)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT iduser,login,password,type FROM users WHERE login = @login;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@login", login);
        //Recupère les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            string? Login = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? Password = reader.IsDBNull(2) ? null : reader.GetString(2);
            string? Type = reader.IsDBNull(3) ? null : reader.GetString(3);
            return new Users(reader.GetInt32(0),Login,Password,Type);
        }
        return null;
    }

    //Inserer user
    public async Task<bool> AddAsync(Users user)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "INSERT INTO users (login,password,type) VALUES (@login,@password,@type);";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@login",(object?)user.Login ?? DBNull.Value);
        // Hachage du mot de passe en une ligne (SHA-256)
        string? hashedPassword = user.Password != null ? Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(user.Password))).ToLower() : null;
        command.Parameters.AddWithValue("@password", (object?)hashedPassword ?? DBNull.Value);
        //
        command.Parameters.AddWithValue("@type",(object?)user.Type ?? DBNull.Value);

        // ExecuteNonQueryAsync s'utilise pour les requêtes qui ne renvoient pas de lignes (INSERT, UPDATE, DELETE)
        var nbr_ligne = await command.ExecuteNonQueryAsync();
        return nbr_ligne > 0;
    }

    //Suppression
    public async Task<bool> DeleteAsync(string login)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "DELETE FROM users WHERE login = @login;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@login", login);

        // ExecuteNonQueryAsync s'utilise pour les requêtes qui ne renvoient pas de lignes (INSERT, UPDATE, DELETE)
        var nbr_ligne = await command.ExecuteNonQueryAsync();
        return nbr_ligne > 0;
    }
}
