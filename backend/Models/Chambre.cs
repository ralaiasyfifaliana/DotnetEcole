namespace GestionHospitaliere.Backend.Models;

public class Chambre
{
    public int? NumChambre{get;}
    public int? Nbr_lit{get;set;}
    public ICollection<Patient?> Patients{get;set;}
    public Chambre(int? numchambre,int? nbr_lit)
    {
        NumChambre = numchambre;
        Nbr_lit = nbr_lit;
        Patients = [];
    }
}