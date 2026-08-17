namespace GestionHospitaliere.Backend.Models;

public class Consultation
{
    public int? RefCons{get;}
    public string? Type{get;set;}
    public DateTime? Date{get;set;}
    public int Frais{get;set;}
    public int? IdMed{get;set;}
    public int? IdPatient{get;set;}
    public string? Prescription{get;set;}
    public string? ObjetFacture{get;set;}
    public string? EtatFacture{get;set;}
    public Consultation(int refcons,string? type,DateTime? date,int frais,int? idmed,int? idpatient,string? prescription,
    string? objetfacture,string? etatfacture)
    {
        RefCons = refcons;
        Type = type;
        Date = date;
        Frais = frais;
        IdMed = idmed;
        IdPatient = idpatient;
        Prescription = prescription;
        ObjetFacture = objetfacture;
        EtatFacture = etatfacture;
    }
}