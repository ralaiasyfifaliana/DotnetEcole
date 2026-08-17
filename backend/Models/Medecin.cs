namespace GestionHospitaliere.Backend.Models;

public class Medecin
{
    public int? Idmed{get;}
    public string? CodeMed {get;set;}
    public string? NomMed{get;set;}
    public string? Poste{get;set;}
    public Medecin(int? idmed,string? codemed,string? nommed,string? poste)
    {
        Idmed = idmed;
        CodeMed = codemed;
        NomMed = nommed;
        Poste = poste;
    }
}