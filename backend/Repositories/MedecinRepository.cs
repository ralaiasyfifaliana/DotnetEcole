namespace GestionHospitaliere.Backend.Repositories;

using GestionHospitaliere.Backend.Data;
using GestionHospitaliere.Backend.Interfaces;
using GestionHospitaliere.Backend.Models;
using Npgsql;
public class MedecinRepository : IMedecinRepository
{
    private readonly Database _database;

    // implémente la connexion à la base de données
    public MedecinRepository(Database database)
    {
        _database = database;
    }

    //Récupère la liste de tous les médecins stockés en base de données
    public async Task<IEnumerable<Medecin?>> GetAllAsync()
    {
        var medecins = new List<Medecin?>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT idmed, codemed, nommed, poste FROM medecin ORDER BY idmed;";
        await using var command = new NpgsqlCommand(sql, connection);

        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? CodeMed = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomMed = reader.IsDBNull(2) ? null : reader.GetString(2);
            string? Poste = reader.IsDBNull(3) ? null : reader.GetString(3);
            medecins.Add(new Medecin(reader.GetInt32(0),CodeMed,NomMed,Poste));
        }

        return medecins;
    }

    // Rechercher un médecin par son code codemed.
    public async Task<Medecin?> GetByIdAsync(string? codemed)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        // Utilisation d'un paramètre (@idmed) pour contrer les injections SQL
        string sql = "SELECT idmed, codemed, nommed, poste FROM medecin WHERE codemed = @codemed;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@codemed", codemed ?? string.Empty);

        //Recupère les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Si une ligne est trouvée, on construit et retourne l'objet Medecin
        if (await reader.ReadAsync())
        {   string? CodeMed = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomMed = reader.IsDBNull(2) ? null : reader.GetString(2);
            string? Poste = reader.IsDBNull(3) ? null : reader.GetString(3);
            return new Medecin(reader.GetInt32(0),CodeMed,NomMed,Poste);
        }

        // Si aucun médecin ne correspond à l'id
        return null;
    }

    //Recuperation medecin par sa poste
    public async Task<IEnumerable<Medecin?>> GetPosAsync(string poste)
    {
        var medecins = new List<Medecin>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT idmed, codemed, nommed, poste FROM medecin WHERE poste = @poste ORDER BY idmed;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@poste",poste);

        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? CodeMed = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomMed = reader.IsDBNull(2) ? null : reader.GetString(2);
            string? Poste = reader.IsDBNull(3) ? null : reader.GetString(3);
            medecins.Add(new Medecin(reader.GetInt32(0),CodeMed,NomMed,Poste));
        }

        return medecins;
    }

    //Recuperation like
    public async Task<IEnumerable<Medecin?>> GetLikeAsync(string valeur)
    {
        var medecins = new List<Medecin>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = @"SELECT idmed, codemed, nommed, poste FROM medecin WHERE codemed LIKE @valeur OR nommed LIKE @valeur OR poste LIKE @valeur;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@valeur",$"%{valeur}%");

        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? CodeMed = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomMed = reader.IsDBNull(2) ? null : reader.GetString(2);
            string? Poste = reader.IsDBNull(3) ? null : reader.GetString(3);
            medecins.Add(new Medecin(reader.GetInt32(0),CodeMed,NomMed,Poste));
        }

        return medecins;
    }

    //Recuperation medecin par like et poste
    public async Task<IEnumerable<Medecin?>> GetLikePosteAsync(string valeur,string poste)
    {
        var medecins = new List<Medecin>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = @"SELECT idmed, codemed, nommed, poste FROM medecin WHERE (codemed LIKE @valeur OR nommed LIKE @valeur OR poste LIKE @valeur) AND poste = @poste;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@valeur",$"%{valeur}%");
        command.Parameters.AddWithValue("@poste",poste);

        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? CodeMed = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomMed = reader.IsDBNull(2) ? null : reader.GetString(2);
            string? Poste = reader.IsDBNull(3) ? null : reader.GetString(3);
            medecins.Add(new Medecin(reader.GetInt32(0),CodeMed,NomMed,Poste));
        }

        return medecins;
    }

    // Insère un nouveau médecin dans la base de données.
    public async Task<bool> AddAsync(Medecin medecin)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "INSERT INTO medecin (codemed,nommed, poste) VALUES (@codemed,@nommed, @poste);";
        await using var command = new NpgsqlCommand(sql, connection);

        // Si la propriété C# est null, on envoie DBNull.Value pour indiquer NULL à PostgreSQL
        var CodeMed = medecin.CodeMed == null ? null : medecin.CodeMed.ToUpper();
        command.Parameters.AddWithValue("@nommed", (object?)medecin.NomMed ?? DBNull.Value);
        command.Parameters.AddWithValue("@poste", (object?)medecin.Poste ?? DBNull.Value);
        command.Parameters.AddWithValue("@codemed", (object?)CodeMed ?? DBNull.Value);

        // ExecuteNonQueryAsync s'utilise pour les requêtes qui ne renvoient pas de lignes (INSERT, UPDATE, DELETE)
        //il renvoie le nombre de ligne affecté
        var nbr_ligne = await command.ExecuteNonQueryAsync();
        return nbr_ligne > 0;
    }

    // Mis à jour
    public async Task<bool> UpdateAsync(Medecin medecin)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "UPDATE medecin SET nommed = @nommed, poste = @poste WHERE codemed = @codemed;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@nommed", (object?)medecin.NomMed ?? DBNull.Value);
        command.Parameters.AddWithValue("@poste", (object?)medecin.Poste ?? DBNull.Value);
        command.Parameters.AddWithValue("@codemed", (object?)medecin.CodeMed ?? DBNull.Value);

        var nbr_ligne = await command.ExecuteNonQueryAsync();
        return nbr_ligne > 0;
    }

    //Supprime un médecin de la base de données à partir de son idmed.
    public async Task<bool> DeleteAsync(string codemd)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "DELETE FROM medecin WHERE codemed = @codemed;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@codemed", codemd);

        var nbr_ligne = await command.ExecuteNonQueryAsync();
        return nbr_ligne > 0;
    }
}
