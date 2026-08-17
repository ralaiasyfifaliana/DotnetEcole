using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Models;
using GestionHospitaliere.Backend.Interfaces;
using System.Xml.Serialization;

namespace GestionHospitaliere.Backend.Services;

public class PatientServices : IPatientServices
{
    private IPatientRepository _patientrepository;
    private IChambreRepository? _chambrerepository;
    public PatientServices(IPatientRepository patientrepository,IChambreRepository? chambreRepository = null)
    {
        _patientrepository = patientrepository;
        _chambrerepository = chambreRepository;
    }
    public async Task<(bool Success, string Message)> InsererPatient(string? numpatient, string? nompatient, string? etat)
    {
        try
        {
            if(numpatient == null || numpatient == "" || numpatient.Trim() == "") return (false, "Numero patient obligatoire");
            if (nompatient == null || nompatient == "" || nompatient.Trim() == "") return (false, "Nom obligatoire");
            if (etat == null) return (false, "Séléctionner l'état du patient");
            if (numpatient != null)
            {
                var patients = await _patientrepository.GetAllAsync();
                List<Patient?> l_patient = patients.ToList();
                foreach (Patient? patient in l_patient)
                {
                    if (patient != null)
                    {
                        if (numpatient.ToUpper() == patient.NumPatient) return (false, "Numero patient déjà existante");
                    }
                }
            }
            Patient patientajout = new Patient(null, numpatient, nompatient, null, null, etat, null);
            if (await _patientrepository.AddAsync(patientajout))
            {
                return (true, "Patient ajouté avec succès");
            }
            return (false, "Erreur d'ajout du patient");
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return (false, "Erreur try");
        }
    }

    public async Task<List<Patient?>> RecherchePatient(string? valeur,string? etat,DateOnly? datehosp,DateOnly? datesortie )
    {
        try
        {
            if(valeur != null && (etat,datehosp,datesortie) == (null, null, null))
            {
                var patient = await _patientrepository.GetByLikeAsync(valeur);
                return patient.ToList();
            }
            if(etat != null && (valeur,datehosp,datesortie) == (null, null, null))
            {
                var patient = await _patientrepository.GetByEtatAsync(etat);
                return patient.ToList();
            }
            if(datehosp != null && (valeur,etat,datesortie) == (null, null, null))
            {
                var patient = await _patientrepository.GetByDateHospAsync(datehosp ?? DateOnly.MinValue);
                return patient.ToList();
            }
            if(datesortie != null && (valeur,etat,datehosp) == (null, null, null))
            {
                var patient = await _patientrepository.GetByDateSortieAsync(datesortie ?? DateOnly.MinValue);
                return patient.ToList();
            }
            if(valeur != null && etat != null && (datehosp,datesortie) == (null, null))
            {
                var patient = await _patientrepository.GetByLikeEtatAsync(valeur,etat);
                return patient.ToList();
            }
            if(datehosp != null && datesortie != null && (valeur,etat) == (null, null))
            {
                var patient = await _patientrepository.GetByDatehospDatesortieAsync(datehosp ?? DateOnly.MinValue, datesortie ?? DateOnly.MinValue);
                return patient.ToList();
            }
            var patients = await _patientrepository.GetByAllAsync(valeur ?? "", etat ?? "", datehosp ?? DateOnly.MinValue, datesortie ?? DateOnly.MinValue);
            return patients.ToList();
            
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return [];
        }
    }

    public async Task<(bool Success, string Message)> DeletePatient(string numpatient)
    {
        try
        {
            if (await _patientrepository.DeleteAsync(numpatient)) return (true, "Patient supprimé avec succès");
            return (false, "Erreur de suppression du patient");
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return (false, "Erreur try");
        }
    }

    public async Task<(bool Success, string Message)> UpdatePatient(string numpatient, string? nompatient, string etat,
        DateOnly? datehosp, DateOnly? datesortie, int? numchambre)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nompatient)) return (false, "Nom et Prenom obligatoire");

            if (etat == "Hospitalisé")
            {
                if (datehosp == null) return (false, "Entrer la date d'hospitalisation");
                if (numchambre == null) return (false, "Veuiller l'affecter à un chambre");
                if(datesortie != null) return (false, "Patient hospitalisé, inutile de preciser la date de sortie");
            }
            else if (etat == "Sortie")
            {
                if (datesortie == null) return (false, "Veuiller mentionner la date de sortie");
                if (datehosp != null && datesortie != null && datehosp > datesortie)
                    return (false, "La date de sortie doit être plus récente que la date d'hospitalisation");
            }
            else
            {
                if (datehosp != null || datesortie != null || numchambre != null)
                    return (false, "Patient non hospitalisé,inutile de preciser les dates et le numero de chambre");
            }

            if(numchambre != null)
            {
                if(_chambrerepository != null)
                {
                    var chambre = await _chambrerepository.GetByIdAsync(numchambre) ?? null;
                    if (chambre != null && chambre.Patients.Count >= chambre.Nbr_lit) return (false, "Chambrer plein");
                }
            }

            Patient patient = new Patient(null, numpatient, nompatient, datehosp, datesortie, etat, numchambre);
            if (await _patientrepository.UpdateAsync(patient))
            {
                return (true, "Patient modifié avec succès");
            }
            return (false, "Erreur de la modification du patient");
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return (false, "Erreur try");
        }
    }
    public async Task<List<Patient?>> ListePatients()
    {
        try
        {
            var patients = await _patientrepository.GetAllAsync();
            return patients.ToList() ?? [];
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return [];
        }
    }
}
