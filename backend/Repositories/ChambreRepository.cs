namespace GestionHospitaliere.Backend.Repositories;

using GestionHospitaliere.Backend.Data;
using GestionHospitaliere.Backend.Interfaces;
using GestionHospitaliere.Backend.Models;
using Npgsql;

public class ChambreRepository : IChambreRepository
{
    private readonly Database _database;

    // implémente la connexion à la base de données
    public ChambreRepository(Database database)
    {
        _database = database;
    }
    //liste des chambre avec leurs patients
    public async Task<IEnumerable<Chambre?>> GetAllAsync()
    {
        var chambres = new Dictionary<int, Chambre>();

        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();
        string sql = @"
        SELECT c.numchambre, c.nbr_lit,p.idpatient,p.numpatient,p.nompatient,p.datehosp,p.datesortie,p.etat,p.numchambre
        FROM chambre c
        LEFT JOIN patient p ON c.numchambre = p.numchambre;";
        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            int numChambre = reader.GetInt32(0);
            //Verification du numero de chambre dans le dico
            if (!chambres.TryGetValue(numChambre, out var chambre))
            {
                chambre = new Chambre(numChambre, reader.GetInt32(1));
                chambres.Add(numChambre, chambre);
            }
            if (!reader.IsDBNull(2))
            {
                string? NumPatient = !reader.IsDBNull(3) ? reader.GetString(3) : null;
                string? NomPatient = !reader.IsDBNull(4) ? reader.GetString(4) : null;
                DateOnly? DateHosp = reader.IsDBNull(5) ? null : reader.GetFieldValue<DateOnly>(5);
                DateOnly? DateSortie = reader.IsDBNull(6) ? null : reader.GetFieldValue<DateOnly>(6);
                string? Etat = reader.IsDBNull(7) ? reader.GetString(7) : null;
                chambre.Patients.Add(new Patient(reader.GetInt32(2),NumPatient,NomPatient,DateHosp,DateSortie,Etat,reader.GetInt32(8)));
            }
        }
        return chambres.Values;
    }
    // Rechercher une chambre par son numero
    public async Task<Chambre?> GetByIdAsync(int? numchambre)
    {
        await using var connection = _database.CreateConnection();
        await connection.OpenAsync();

        string sql = @"
        SELECT c.numchambre, c.nbr_lit,p.idpatient,p.numpatient,p.nompatient,p.datehosp,p.datesortie,p.etat,p.numchambre
        FROM chambre c
        LEFT JOIN patient p ON c.numchambre = p.numchambre
        WHERE c.numchambre = @numchambre;";
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("@numchambre", numchambre??0);
        //Recupère les valeurs de la commande sql
        await using var reader = await command.ExecuteReaderAsync();
        Chambre? chambre = null;
        while(await reader.ReadAsync())
        {
            chambre ??= new(reader.GetInt32(0),reader.GetInt32(1));

            if (!reader.IsDBNull(2))
            {
                string? NumPatient = !reader.IsDBNull(3) ? reader.GetString(3) : null;
                string? NomPatient = !reader.IsDBNull(4) ? reader.GetString(4) : null;
                DateOnly? DateHosp = reader.IsDBNull(5) ? null : reader.GetFieldValue<DateOnly>(5);
                DateOnly? DateSortie = reader.IsDBNull(6) ? null : reader.GetFieldValue<DateOnly>(6);
                string? Etat = reader.IsDBNull(7) ? reader.GetString(7) : null;
                chambre.Patients.Add(new Patient(reader.GetInt32(2),NumPatient,NomPatient,DateHosp,DateSortie,Etat,reader.GetInt32(8)));
            }

        }
        return chambre;
    }

}
