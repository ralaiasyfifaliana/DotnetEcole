namespace GestionHospitaliere.Backend.Models;

public class Patient
{
    public int? IdPatient{get;}
    public string? NumPatient{get;set;}
    public string? NomPatient{get;set;}
    public string? Etat{get;set;}
    public DateOnly? DateHosp{get;set;}
    public DateOnly? DateSortie{get;set;}
    public int? NumChambre{get;set;}
    public Patient(int? idpatient=null,string? numpatient = null,string? nompatient = null,DateOnly? datehosp = null,
    DateOnly? datesortie = null,string? etat = null,int? numchambre = null)
    {
        IdPatient = idpatient;
        NumPatient = numpatient;
        NomPatient = nompatient;
        DateHosp = datehosp;
        DateSortie = datesortie;
        Etat = etat;
        NumChambre = numchambre;
    }
}