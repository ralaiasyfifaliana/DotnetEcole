using System.Reflection.Metadata.Ecma335;

namespace GestionHospitaliere.Backend.Repositories;

using GestionHospitaliere.Backend.Data;
using GestionHospitaliere.Backend.Interfaces;
using GestionHospitaliere.Backend.Models;
using Npgsql;

public class ConsultationRepository : IConsultationRepository
{
    private readonly Database _database;

    // implémente la connexion à la base de données
    public ConsultationRepository(Database database)
    {
        _database = database;
    }

    //Récupère la liste de tous les consultation stockés en base de données
    public async Task<IEnumerable<Consultation?>> GetAllAsync()
    {
        var consultation = new List<Consultation?>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture FROM consultation;";
        await using var command = new NpgsqlCommand(sql, connection);
        
        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            consultation.Add(new Consultation(reader.GetInt32(0),Type,Date,reader.GetInt32(3),reader.GetInt32(4),reader.GetInt32(5),Prescription,ObjetFacture,EtatFacture));
        }

        return consultation;
    }

    // Rechercher une consultation par son refcons
    public async Task<Consultation?> GetByIdAsync(int refcons)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture FROM consultation WHERE refcons = @refcons;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@refcons", refcons);

        //Recupère les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Si une ligne est trouvée, on construit et retourne l'objet Consultation
        if (await reader.ReadAsync())
        {   
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            return new Consultation(reader.GetInt32(0),Type,Date,reader.GetInt32(3),reader.GetInt32(4),reader.GetInt32(5),Prescription,ObjetFacture,EtatFacture);
        }
        // Si aucun consultation ne correspond au reference
        return null; 
    }

    //Recupere les consultation par type
    public async Task<IEnumerable<Consultation?>> GetByTypeAsync(string type)
    {
        var consultation = new List<Consultation?>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture FROM consultation WHERE type = @type;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@type",type);
        
        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            int? IdMed = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            consultation.Add(new Consultation(reader.GetInt32(0),Type,Date,reader.GetInt32(3),IdMed,reader.GetInt32(5),Prescription,ObjetFacture,EtatFacture));
        }

        return consultation;
    }

    //Recuperer les consultation par medecin
    public async Task<IEnumerable<Consultation?>> GetByMedAsync(int idmed)
    {
        var consultation = new List<Consultation? >();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture FROM consultation WHERE idmed = @idmed;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@idmed",idmed);
        
        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            int? IdMed = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            consultation.Add(new Consultation(reader.GetInt32(0),Type,Date,reader.GetInt32(3),IdMed,reader.GetInt32(5),Prescription,ObjetFacture,EtatFacture));
        }

        return consultation;
    }
    
    //Recuperer les consultation par patient
    public async Task<IEnumerable<Consultation?>> GetByPatAsync(int idpatient)
    {
        var consultation = new List<Consultation>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture FROM consultation WHERE idpatient = @idpatient;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@idpatient",idpatient);
        
        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            int? IdMed = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            consultation.Add(new Consultation(reader.GetInt32(0),Type,Date,reader.GetInt32(3),IdMed,reader.GetInt32(5),Prescription,ObjetFacture,EtatFacture));
        }

        return consultation;
    }
    
    //Recuperer les consultations par date
    public async Task<IEnumerable<Consultation?>>GetByDateAsync(DateOnly? date)
    {
        var consultation = new List<Consultation>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture FROM consultation WHERE date::date = @date;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@date",date ?? DateOnly.MinValue);
        
        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            int? IdMed = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            consultation.Add(new Consultation(reader.GetInt32(0),Type,Date,reader.GetInt32(3),IdMed,
            reader.GetInt32(5),Prescription,ObjetFacture,EtatFacture));
        }

        return consultation;
    }
    
    //Recuperer les consultation par medecin et par date
    public async Task<IEnumerable<Consultation?>> GetByMedDateAsync(int idmed, DateOnly? date)
    {
        var consultation = new List<Consultation>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture FROM consultation WHERE idmed = @idmed AND date::date = @date;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@idmed",idmed);
        command.Parameters.AddWithValue("@date",date ?? DateOnly.MinValue);
        
        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            int? IdMed = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            consultation.Add(new Consultation(reader.GetInt32(0),Type,Date,reader.GetInt32(3),IdMed,
            reader.GetInt32(5),Prescription,ObjetFacture,EtatFacture));
        }

        return consultation;
    }
    
    //Recuperer les consultation par type et par date
    public async Task<IEnumerable<Consultation?>> GetByTypeDateAsync(string type, DateOnly? date)
    {
        var consultation = new List<Consultation>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture FROM consultation WHERE type = @type AND date::date = @date;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@type",type);
        command.Parameters.AddWithValue("@date",date ?? DateOnly.MinValue);
        
        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            int? IdMed = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            consultation.Add(new Consultation(reader.GetInt32(0),Type,Date,reader.GetInt32(3),IdMed,
            reader.GetInt32(5),Prescription,ObjetFacture,EtatFacture));
        }

        return consultation;
    }
    
    //Recuperer consultation entre 2 dates
    public async Task<IEnumerable<Consultation?>> GetBetweenAsync(DateOnly? date1, DateOnly? date2)
    {
        var consultation = new List<Consultation>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture FROM consultation WHERE date::date BETWEEN @date1 AND @date2;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@date1",date1 ?? DateOnly.MinValue);
        command.Parameters.AddWithValue("@date2",date2 ?? DateOnly.MinValue);
        
        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            int? IdMed = reader.IsDBNull(4) ? null : reader.GetInt32(4);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            consultation.Add(new Consultation(reader.GetInt32(0),Type,Date,reader.GetInt32(3),IdMed,
            reader.GetInt32(5),Prescription,ObjetFacture,EtatFacture));
        }

        return consultation;
    }
    
    // Insère une nouvelle Consultation dans la base de données.
    public async Task<bool> AddAsync(Consultation consultation)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = @"INSERT INTO consultation (type, date,frais,idmed,idpatient,prescription,objetfacture,etatfacture) 
        VALUES (@type,@date,@frais,@idmed,@idpatient,@prescription,@objetfacture,@etatfacture);";
        await using var command = new NpgsqlCommand(sql, connection);

        // Si la propriété C# est null, on envoie DBNull.Value pour indiquer NULL à PostgreSQL
        command.Parameters.AddWithValue("@type", (object?)consultation.Type ?? DBNull.Value);
        command.Parameters.AddWithValue("@date", (object?)consultation.Date ?? DBNull.Value);
        command.Parameters.AddWithValue("@frais", (object?)consultation.Frais ?? DBNull.Value);
        command.Parameters.AddWithValue("@idmed", (object?)consultation.IdMed ?? DBNull.Value);
        command.Parameters.AddWithValue("@idpatient", (object?)consultation.IdPatient ?? DBNull.Value);
        command.Parameters.AddWithValue("@prescription", (object?)consultation.Prescription ?? DBNull.Value);
        command.Parameters.AddWithValue("@objetfacture",(object?)consultation.ObjetFacture ?? DBNull.Value);
        command.Parameters.AddWithValue("@etatfacture",(object?)consultation.EtatFacture ?? DBNull.Value);
        // ExecuteNonQueryAsync s'utilise pour les requêtes qui ne renvoient pas de lignes (INSERT, UPDATE, DELETE)
        var nbr_ligne = await command.ExecuteNonQueryAsync();
        return nbr_ligne > 0;
    }
    // Mis à jour
    public async Task<bool> UpdateAsync(Consultation consultation)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = @"UPDATE consultation SET type = @type, date = @date, frais = @frais, idmed = @idmed, idpatient = @idpatient,
        prescription = @prescription,objetfacture = @objetfacture,etatfacture = @etatfacture WHERE refcons = @refcons;";
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@refcons", (object?)consultation.RefCons ?? DBNull.Value);
        command.Parameters.AddWithValue("@type", (object?)consultation.Type ?? DBNull.Value);
        command.Parameters.AddWithValue("@date", (object?)consultation.Date ?? DBNull.Value);
        command.Parameters.AddWithValue("@frais", (object?)consultation.Frais ?? DBNull.Value);
        command.Parameters.AddWithValue("@idmed", (object?)consultation.IdMed ?? DBNull.Value);
        command.Parameters.AddWithValue("@idpatient", (object?)consultation.IdPatient ?? DBNull.Value);
        command.Parameters.AddWithValue("@prescription", (object?)consultation.Prescription ?? DBNull.Value);
        command.Parameters.AddWithValue("@objetfacture",(object?)consultation.ObjetFacture ?? DBNull.Value);
        command.Parameters.AddWithValue("@etatfacture",(object?)consultation.EtatFacture ?? DBNull.Value);

        var nbr_ligne = await command.ExecuteNonQueryAsync();
        return nbr_ligne > 0;
    }

    //Supprime une consultation de la base de données à partir de son refcons
    public async Task<bool> DeleteAsync(int refcons)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "DELETE FROM consultation WHERE refcons = @refcons;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@refcons", refcons);

        var nbr_ligne = await command.ExecuteNonQueryAsync();
        return nbr_ligne > 0;
    }

    public async Task<IEnumerable<Consultation?>> GetDefaultAsync(int idmed, DateOnly? date)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();
        var consultations = new List<Consultation?>();

        string sql = @"SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture 
        FROM consultation WHERE idmed = @idmed AND date::date = @date AND objetfacture IS NULL;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@idmed", idmed);
        command.Parameters.AddWithValue("@date", date??DateOnly.MinValue);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            consultations.Add(new Consultation(reader.GetInt32(0),Type,Date,reader.GetInt32(3),reader.GetInt32(4),reader.GetInt32(5),Prescription,ObjetFacture,EtatFacture));
        }
        return consultations;
    }

    public async Task<IEnumerable<Consultation?>> GetFinishAsync(int idmed, DateOnly? date)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();
        var consultations = new List<Consultation?>();

        string sql = @"SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture 
        FROM consultation WHERE idmed = @idmed AND date::date = @date AND objetfacture IS NOT NULL;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@idmed", idmed);
        command.Parameters.AddWithValue("@date", date??DateOnly.MinValue);

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            consultations.Add(new Consultation(reader.GetInt32(0),Type,Date,reader.GetInt32(3),reader.GetInt32(4),reader.GetInt32(5),Prescription,ObjetFacture,EtatFacture));
        }
        return consultations;
    }

    public async Task<IEnumerable<Consultation?>> GetBetweenByMedAsync(int idmed, DateOnly? date1, DateOnly? date2)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();
        var consultations = new List<Consultation?>();
        string sql = @"SELECT refcons, type, date, frais, idmed, idpatient,prescription,objetfacture,etatfacture 
        FROM consultation WHERE idmed = @idmed AND date::date BETWEEN @date1 AND @date2;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@idmed", idmed);
        command.Parameters.AddWithValue("@date1", date1 ?? DateOnly.MinValue);
        command.Parameters.AddWithValue("@date2", date2 ?? DateOnly.MinValue);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string? Type = reader.IsDBNull(1) ? null : reader.GetString(1);
            DateTime? Date = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            string? Prescription = reader.IsDBNull(6) ? null : reader.GetString(6);
            string? ObjetFacture = reader.IsDBNull(7) ? null : reader.GetString(7);
            string? EtatFacture = reader.IsDBNull(8) ? null : reader.GetString(8);
            consultations.Add(new Consultation(reader.GetInt32(0), Type, Date, reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5), Prescription, ObjetFacture, EtatFacture));
        }
        return consultations;
    }
}