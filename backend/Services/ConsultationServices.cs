using GestionHospitaliere.Backend.Interfaces;
using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Models;

namespace GestionHospitaliere.Backend.Services;

public class ConsultationServices : IConsultationServices
{
    private readonly IConsultationRepository _consultationRepository;
    private readonly IMedecinRepository? _medecinRepository;
    private readonly IPatientRepository? _patientRepository;
    public ConsultationServices(IConsultationRepository consultationRepository, IMedecinRepository? medecinRepository = null, IPatientRepository? patientRepository = null)
    {
        _consultationRepository = consultationRepository;
        _medecinRepository = medecinRepository;
        _patientRepository = patientRepository;
    }
    public async Task<List<Consultation?>> ListeConsultation()                             
    {
        try
        {
            var consultations = await _consultationRepository.GetAllAsync();
            return consultations.ToList() ?? [];
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Erreur lors de la récupération des consultations : {exc.Message}");
            return [];
        }
    }

    public async Task<List<Consultation?>> ListConsMedecin(int idmed,DateOnly? date)
    {
        try
        {
            var consultations = await _consultationRepository.GetDefaultAsync(idmed,date??DateOnly.MinValue);
            return consultations.ToList() ?? [];
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Erreur lors de la récupération des consultations du médecin : {exc.Message}");
            return [];
        }
    }

    public async Task<(bool Success, string Message)> PlanifierConsultation(string? type, DateTime? date, int? frais, string? codemedecin, string? numpatient)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(type)) return (false, "Le type de consultation est requis.");
            if(date == null) return (false, "La date et l'heure de consultation est requise.");
            if(frais < 0) return (false, "Les frais de consultation doivent être supérieurs ou égaux à zéro.");
            if(string.IsNullOrWhiteSpace(codemedecin)) return (false, "Le code du médecin est requis.");
            if(string.IsNullOrWhiteSpace(numpatient)) return (false, "L'identifiant du patient est requis.");
            if(_medecinRepository != null && _patientRepository != null)
            {
                var medecin = await _medecinRepository.GetByIdAsync(codemedecin);
                var patient = await _patientRepository.GetByIdAsync(numpatient);
                if(medecin != null && patient != null)
                {
                    Consultation consultation = new Consultation(0, type, date, frais ?? 0, medecin.Idmed, patient.IdPatient, null, null, "Non-Payé");
                    if(await _consultationRepository.AddAsync(consultation))
                    {
                        return (true, "Consultation planifiée avec succès.");
                    }
                }
            }
            return (false, "Erreur lors de la planification de la consultation.");
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Erreur lors de la planification de la consultation : {exc.Message}");
            return (false, $"{exc.Message}");
        }
    }

    public async Task<(bool Success, string Message)> ModifierConsultation(int? refcons,string? type, DateTime? date, int? frais, string? codemedecin, string? numpatient, string? prescription, string? objetfacture, string? etatfacture)
    {
        try
        {
            if(string.IsNullOrWhiteSpace(type)) return (false, "Le type de consultation est requis.");
            if(date == null) return (false, "La date et l'heure de consultation est requise.");
            if(frais < 0) return (false, "Les frais de consultation doivent être supérieurs ou égaux à zéro.");
            if(string.IsNullOrWhiteSpace(codemedecin)) return (false, "Le code du médecin est requis.");
            if(string.IsNullOrWhiteSpace(numpatient)) return (false, "L'identifiant du patient est requis.");
            if(_medecinRepository != null && _patientRepository != null)
            {
                var medecin = await _medecinRepository.GetByIdAsync(codemedecin);
                var patient = await _patientRepository.GetByIdAsync(numpatient);
                if(medecin != null && patient != null)
                {
                    Consultation consultation = new Consultation(refcons??0, type, date, frais ?? 0, medecin.Idmed, patient.IdPatient, prescription, objetfacture, etatfacture);
                    if(await _consultationRepository.UpdateAsync(consultation))
                    {
                        return (true, "Consultation modifiée avec succès.");
                    }
                }
            }
            return (false, "Consultation non existante");
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Erreur lors de la modification de la consultation : {exc.Message}");
            return (false, $"Erreur lors de la modification de la consultation : {exc.Message}");
        }
    }
    public async Task<(bool Success, string Message)> AnnulerConsultation(int refcons)
    {
        try
        {
            if(await _consultationRepository.DeleteAsync(refcons))
            {
                return (true, "Consultation annulée avec succès.");
            }
            return (false, "Erreur try"); 
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Erreur lors de l'annulation de la consultation : {exc.Message}");
            return (false, $"Erreur lors de l'annulation de la consultation : {exc.Message}");
        }
    }

    public async Task<List<Consultation?>> RechercheConsultation(string? type, DateOnly? date1, DateOnly? date2, string? codemedecin, string? numpatient)
    {
        try
        {
            IEnumerable<Consultation?>? result = null;

            // date1 ou date2 
            DateOnly? dateUnique = (date1 != null ^ date2 != null) ? (date1 ?? date2) : null;

            // 1. Recherche par Type seul
            if (type != null && date1 == null && date2 == null && codemedecin == null && numpatient == null)
            {
                result = await _consultationRepository.GetByTypeAsync(type);
            }
            // 2. Recherche par Date seule (date1 OU date2)
            else if (dateUnique != null && type == null && codemedecin == null && numpatient == null)
            {
                result = await _consultationRepository.GetByDateAsync(dateUnique);
            }
            // 3. Recherche entre deux dates
            else if (date1 != null && date2 != null && type == null && codemedecin == null && numpatient == null)
            {
                result = await _consultationRepository.GetBetweenAsync(date1, date2);
            }
            // 4. Recherche par Type + Date seule
            else if (type != null && dateUnique != null && codemedecin == null && numpatient == null)
            {
                result = await _consultationRepository.GetByTypeDateAsync(type, dateUnique);
            }
            // 5. Recherche par Médecin + Date seule
            else if (codemedecin != null && dateUnique != null && type == null && numpatient == null)
            {
                if (_medecinRepository != null)
                {
                    var medecin = await _medecinRepository.GetByIdAsync(codemedecin);
                    if (medecin != null)
                        result = await _consultationRepository.GetByMedDateAsync(medecin.Idmed ?? 0, dateUnique);
                }
            }
            // 6. Recherche par Médecin seul
            else if (codemedecin != null && date1 == null && date2 == null && type == null && numpatient == null)
            {
                if (_medecinRepository != null)
                {
                    var medecin = await _medecinRepository.GetByIdAsync(codemedecin);
                    if (medecin != null)
                        result = await _consultationRepository.GetByMedAsync(medecin.Idmed ?? 0);
                }
            }
            // 7. Recherche par Patient seul
            else if (numpatient != null && date1 == null && date2 == null && type == null && codemedecin == null)
            {
                if (_patientRepository != null)
                {
                    var patient = await _patientRepository.GetByIdAsync(numpatient);
                    if (patient != null)
                        result = await _consultationRepository.GetByPatAsync(patient.IdPatient ?? 0);
                }
            }
            //8.Recherche par medecin et entre deux dates
            else if (codemedecin != null && date1 != null && date2 != null && type == null && numpatient == null)
            {
                if (_medecinRepository != null)
                {
                    var medecin = await _medecinRepository.GetByIdAsync(codemedecin);
                    if (medecin != null)
                        result = await _consultationRepository.GetBetweenByMedAsync(medecin.Idmed ?? 0, date1, date2);
                }
            }

            return result?.ToList() ?? [];
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Erreur lors de la recherche de la consultation : {exc.Message}");
            return [];
        }
    }

    public async Task<List<Consultation?>> ListConsAchevee(int idmed, DateOnly? date)
    {
        try
        {
            var consultations = await _consultationRepository.GetFinishAsync(idmed, date ?? DateOnly.MinValue);
            return  consultations.ToList() ?? [];
        }
        catch (Exception exc)
        {
            Console.WriteLine($"Erreur lors de la récupération des consultations achevées du médecin : {exc.Message}");
            return [];
        }
    }
}

