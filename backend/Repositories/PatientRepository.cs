namespace GestionHospitaliere.Backend.Repositories;

using GestionHospitaliere.Backend.Data;
using GestionHospitaliere.Backend.Interfaces;
using GestionHospitaliere.Backend.Models;
using Npgsql;

public class PatientRepository : IPatientRepository
{
    private readonly Database _database;

    // implémente la connexion à la base de données
    public PatientRepository(Database database)
    {
        _database = database;
    }

    //Récupère la liste de tous les patient stockés en base de données
    public async Task<IEnumerable<Patient?>> GetAllAsync()
    {
        var patient = new List<Patient>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT idpatient, numpatient, nompatient, datehosp, datesortie, etat, numchambre FROM patient;";
        await using var command = new NpgsqlCommand(sql, connection);

        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? NumPatient = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomPatient = reader.IsDBNull(2) ? null : reader.GetString(2);
            DateOnly? DateHosp = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
            DateOnly? DateSortie = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateOnly>(4);
            string? Etat = reader.IsDBNull(5) ? null : reader.GetString(5);
            int? NumChambre = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            patient.Add(new Patient(reader.GetInt32(0),NumPatient,NomPatient,DateHosp,DateSortie,Etat,NumChambre));
        }

        return patient;
    }

    // Rechercher un patient par son numero
    public async Task<Patient?> GetByIdAsync(string numpatient)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT idpatient, numpatient, nompatient, datehosp, datesortie, etat, numchambre FROM patient WHERE numpatient = @numpatient;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@numpatient", numpatient);

        //Recupère les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Si une ligne est trouvée, on construit et retourne l'objet Patient
        if (await reader.ReadAsync())
        {
            string? NumPatient = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomPatient = reader.IsDBNull(2) ? null : reader.GetString(2);
            DateOnly? DateHosp = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
            DateOnly? DateSortie = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateOnly>(4);
            string? Etat = reader.IsDBNull(5) ? null : reader.GetString(5);
            int? NumChambre = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            return new Patient(reader.GetInt32(0),NumPatient,NomPatient,DateHosp,DateSortie,Etat,NumChambre);
        }

        // Si aucun patient ne correspond à l'id
        return null;
    }

    //Recuperer patient par etat
    public async Task<IEnumerable<Patient?>> GetByEtatAsync(string etat)
    {
        var patient = new List<Patient>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT idpatient, numpatient, nompatient, datehosp, datesortie, etat, numchambre FROM patient WHERE etat = @etat;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@etat", etat);
        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? NumPatient = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomPatient = reader.IsDBNull(2) ? null : reader.GetString(2);
            DateOnly? DateHosp = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
            DateOnly? DateSortie = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateOnly>(4);
            string? Etat = reader.IsDBNull(5) ? null : reader.GetString(5);
            int? NumChambre = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            patient.Add(new Patient(reader.GetInt32(0),NumPatient,NomPatient,DateHosp,DateSortie,Etat,NumChambre));
        }

        return patient;
    }

    //Recuperer patient par like
    public async Task<IEnumerable<Patient?>> GetByLikeAsync(string valeur)
    {
        var patient = new List<Patient>();

        //connexion,et fermeture automatique de la base de données
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = @"SELECT idpatient, numpatient, nompatient, datehosp, datesortie, etat, numchambre FROM patient 
        WHERE numpatient LIKE @valeur OR nompatient LIKE @valeur OR etat LIKE @valeur OR numchambre::text LIKE @valeur;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@valeur",$"%{valeur}%");
        // Recuperer les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Parcourt les résultats ligne par ligne
        while (await reader.ReadAsync())
        {
            string? NumPatient = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomPatient = reader.IsDBNull(2) ? null : reader.GetString(2);
            DateOnly? DateHosp = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
            DateOnly? DateSortie = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateOnly>(4);
            string? Etat = reader.IsDBNull(5) ? null : reader.GetString(5);
            int? NumChambre = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            patient.Add(new Patient(reader.GetInt32(0),NumPatient,NomPatient,DateHosp,DateSortie,Etat,NumChambre));
        }

        return patient;
    }

    //Recuperer patient par datehosp
    public async Task<IEnumerable<Patient?>> GetByDateHospAsync(DateOnly date)
    {
        var patient = new List<Patient>();

        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT idpatient, numpatient, nompatient, datehosp, datesortie, etat, numchambre FROM patient WHERE datehosp::date = @date;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@date", date);

        //Recupère les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Si une ligne est trouvée, on construit et retourne l'objet Patient
        if (await reader.ReadAsync())
        {
            string? NumPatient = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomPatient = reader.IsDBNull(2) ? null : reader.GetString(2);
            DateOnly? DateHosp = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
            DateOnly? DateSortie = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateOnly>(4);
            string? Etat = reader.IsDBNull(5) ? null : reader.GetString(5);
            int? NumChambre = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            patient.Add(new Patient(reader.GetInt32(0),NumPatient,NomPatient,DateHosp,DateSortie,Etat,NumChambre));
        }
        return patient;
    }

    //Recuperer patient par datesortie
    public async Task<IEnumerable<Patient?>> GetByDateSortieAsync(DateOnly date)
    {
        var patient = new List<Patient>();

        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT idpatient, numpatient, nompatient, datehosp, datesortie, etat, numchambre FROM patient WHERE datehosp::date = @date;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@date", date );

        //Recupère les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Si une ligne est trouvée, on construit et retourne l'objet Patient
        if (await reader.ReadAsync())
        {
            string? NumPatient = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomPatient = reader.IsDBNull(2) ? null : reader.GetString(2);
            DateOnly? DateHosp = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
            DateOnly? DateSortie = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateOnly>(4);
            string? Etat = reader.IsDBNull(5) ? null : reader.GetString(5);
            int? NumChambre = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            patient.Add(new Patient(reader.GetInt32(0),NumPatient,NomPatient,DateHosp,DateSortie,Etat,NumChambre));
        }
        return patient;
    }

    //recuperer par datehosp et datesortie
    public async Task<IEnumerable<Patient?>> GetByDatehospDatesortieAsync(DateOnly datehosp,DateOnly datesortie)
    {
        var patient = new List<Patient>();

        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "SELECT idpatient, numpatient, nompatient, datehosp, datesortie, etat, numchambre FROM patient WHERE datehosp::date = @datehosp AND datesortie::date = @datesortie;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@datehosp", datehosp);
        command.Parameters.AddWithValue("@datesortie", datesortie);

        //Recupère les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();

        // Si une ligne est trouvée, on construit et retourne l'objet Patient
        if (await reader.ReadAsync())
        {
            string? NumPatient = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomPatient = reader.IsDBNull(2) ? null : reader.GetString(2);
            DateOnly? DateHosp = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
            DateOnly? DateSortie = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateOnly>(4);
            string? Etat = reader.IsDBNull(5) ? null : reader.GetString(5);
            int? NumChambre = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            patient.Add(new Patient(reader.GetInt32(0),NumPatient,NomPatient,DateHosp,DateSortie,Etat,NumChambre));
        }
        return patient;
    }
    //recuperer par like et par etat
    public async Task<IEnumerable<Patient?>> GetByLikeEtatAsync(string valeur, string etat)
    {
        var patient = new List<Patient>();

        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = @"SELECT idpatient, numpatient, nompatient, datehosp, datesortie, etat, numchambre FROM patient 
        WHERE (numpatient LIKE @valeur OR nompatient LIKE @valeur OR numchambre::text LIKE @valeur) AND etat = @etat;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@valeur", $"%{valeur ?? string.Empty }%");
        command.Parameters.AddWithValue("@etat", etat ?? (object)DBNull.Value);

        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            string? NumPatient = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomPatient = reader.IsDBNull(2) ? null : reader.GetString(2);
            DateOnly? DateHosp = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
            DateOnly? DateSortie = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateOnly>(4);
            string? Etat = reader.IsDBNull(5) ? null : reader.GetString(5);
            int? NumChambre = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            patient.Add(new Patient(reader.GetInt32(0),NumPatient,NomPatient,DateHosp,DateSortie,Etat,NumChambre));
        }
        return patient;
    }
    //recuperer par tout
    public async Task<IEnumerable<Patient?>> GetByAllAsync(string valeur,string etat, DateOnly datehosp, DateOnly datesortie)
    {
        var patient = new List<Patient>();
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();
        string sql = @"SELECT idpatient, numpatient, nompatient, datehosp, datesortie, etat, numchambre FROM patient 
        WHERE (numpatient LIKE @valeur OR nompatient LIKE @valeur OR numchambre::text LIKE @valeur) AND etat = @etat AND datehosp::date = @datehosp AND datesortie::date = @datesortie;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@valeur", $"%{valeur}%");
        command.Parameters.AddWithValue("@etat", etat);
        command.Parameters.AddWithValue("@datehosp", datehosp);
        command.Parameters.AddWithValue("@datesortie", datesortie);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            string? NumPatient = reader.IsDBNull(1) ? null : reader.GetString(1);
            string? NomPatient = reader.IsDBNull(2) ? null : reader.GetString(2);
            DateOnly? DateHosp = reader.IsDBNull(3) ? null : reader.GetFieldValue<DateOnly>(3);
            DateOnly? DateSortie = reader.IsDBNull(4) ? null : reader.GetFieldValue<DateOnly>(4);
            string? Etat = reader.IsDBNull(5) ? null : reader.GetString(5);
            int? NumChambre = reader.IsDBNull(6) ? null : reader.GetInt32(6);
            patient.Add(new Patient(reader.GetInt32(0),NumPatient,NomPatient,DateHosp,DateSortie,Etat,NumChambre));
        }
        return patient;
    }
    
    // Insère un nouveau Patient dans la base de données.
    public async Task<bool> AddAsync(Patient patient)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "INSERT INTO patient (numpatient,nompatient, etat,datehosp,datesortie,numchambre) VALUES (@numpatient,@nompatient, @etat,@datehosp,@datesortie,@numchambre);";
        await using var command = new NpgsqlCommand(sql, connection);

        // Si la propriété C# est null, on envoie DBNull.Value pour indiquer NULL à PostgreSQL
        command.Parameters.AddWithValue("@nompatient", (object?)patient.NomPatient ?? DBNull.Value);
        command.Parameters.AddWithValue("@etat", (object?)patient.Etat ?? DBNull.Value);
        command.Parameters.AddWithValue("@datehosp", (object?)patient.DateHosp ?? DBNull.Value);
        command.Parameters.AddWithValue("@datesortie", (object?)patient.DateSortie ?? DBNull.Value);
        command.Parameters.AddWithValue("@numchambre", (object?)patient.NumChambre ?? DBNull.Value);
        command.Parameters.AddWithValue("@numpatient",(object?)patient.NumPatient == null ? DBNull.Value : patient.NumPatient.ToUpper());
        // ExecuteNonQueryAsync s'utilise pour les requêtes qui ne renvoient pas de lignes (INSERT, UPDATE, DELETE)
        var nbr_ligne = await command.ExecuteNonQueryAsync();
        return nbr_ligne > 0;
    }
    // Mis à jour
    public async Task<bool> UpdateAsync(Patient patient)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "UPDATE patient SET nompatient = @nompatient, etat = @etat, datehosp = @datehosp, datesortie = @datesortie, numchambre = @numchambre WHERE numpatient = @numpatient;";
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("@nompatient", (object?)patient.NomPatient ?? DBNull.Value);
        command.Parameters.AddWithValue("@datehosp", (object?)patient.DateHosp ?? DBNull.Value);
        command.Parameters.AddWithValue("@datesortie", (object?)patient.DateSortie ?? DBNull.Value);
        command.Parameters.AddWithValue("@etat", (object?)patient.Etat ?? DBNull.Value);
        command.Parameters.AddWithValue("@numchambre", (object?)patient.NumChambre ?? DBNull.Value);
        command.Parameters.AddWithValue("@numpatient",(object?)patient.NumPatient ?? DBNull.Value);

        var nbr_ligne = await command.ExecuteNonQueryAsync();
        return nbr_ligne > 0;
    }
    //Supprime un patient de la base de données à partir de son idpatient.
    public async Task<bool> DeleteAsync(string numpatient)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = "DELETE FROM patient WHERE numpatient = @numpatient;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@numpatient", numpatient);

        var nbr_ligne = await command.ExecuteNonQueryAsync();
        return nbr_ligne > 0;
    }

}
