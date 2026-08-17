using GestionHospitaliere.Backend.Models;

namespace GestionHospitaliere.Backend.Interfaces.Services;

public interface IConsultationServices
{
    //planifier une consultation
    Task<(bool Success, string Message)> PlanifierConsultation(string? type, DateTime? date, int? frais, string? codemedecin, string? numpatient);
    //modifier une consultation
    Task<(bool Success, string Message)> ModifierConsultation(int? refcons,string? type, DateTime? date, int? frais, string? codemedecin, string? numpatient, string? prescription, string? objetfacture, string? etatfacture);
    //annuler une consultation
    Task<(bool Success, string Message)> AnnulerConsultation(int refcons);
    //liste consultation
    Task<List<Consultation?>> ListeConsultation();
    //liste consultation par defaut session medecin
    Task<List<Consultation?>> ListConsMedecin(int idmed, DateOnly? date);
    //consultation achevée
    Task<List<Consultation?>> ListConsAchevee(int idmed, DateOnly? date);
    //recherche consultation
    Task<List<Consultation?>> RechercheConsultation(string? type,DateOnly? date1,DateOnly? date2,string? codemedecin, string? numpatient);
}