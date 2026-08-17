namespace GestionHospitaliere.Backend.Services;

using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Models;
using GestionHospitaliere.Backend.Interfaces;

public class MedecinServices : IMedecinServices
{
    private IMedecinRepository _medecinrepository;
    private IUsersRepository? _usersrepository;
    public MedecinServices(IMedecinRepository medecinrepository,IUsersRepository? usersrepository = null)
    {
        _medecinrepository = medecinrepository;
        _usersrepository = usersrepository;
    }

    public async Task<List<Medecin?>> ListeMedecin()
    {
        try
        {
            var medecins = await _medecinrepository.GetAllAsync();
            return medecins.ToList();
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return [];
        }
    }

    public async Task<(bool Success, string Message)> InsererMedecin(string? codemed, string? nommed, string? poste,string? password,string? type)
    {
        try
        {
            if(codemed == null || codemed == "" || codemed.Trim() == "") return(false,"Code medecin obligatoire");
            if(nommed == null || nommed == "" || nommed.Trim() == "") return (false,"Nom obligatoire");
            if(poste == null) return (false,"Choisir une poste");
            if(password == null || password == "" || password.Trim() == "") return (false,"Mot de passe obligatoire");
            if(codemed != null)
            {
                var medecins = await _medecinrepository.GetAllAsync();
                List<Medecin?> l_medecins = medecins.ToList();
                foreach(Medecin? medecin in l_medecins)
                {
                    if(medecin != null)
                    {
                        if(medecin.CodeMed == codemed.Trim().ToUpper()) return(false,"Code Medecin déjà existante");
                        if(medecin.NomMed == nommed && medecin.Poste == poste) return(false,"Le Medecin existe déjà");
                    }
                }
            }
            Medecin medecinajout = new Medecin(null,codemed == null ? "" : codemed.Trim().ToUpper(),nommed,poste);
            if(await _medecinrepository.AddAsync(medecinajout))
            {
                if(_usersrepository != null)
                {
                    var user = new UsersServices(_usersrepository);
                    var command = await user.InsererUtilisateur(codemed,password,type);
                    if (command.Success)
                    {
                        return (true,"Medecin ajouté avec succès");
                    }
                }
            }
            return (false,"Erreur try");
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return (false,$"Error: {exc}");
        }
    }

    public async Task<List<Medecin?>> RechercheMedecin(string? value,string? poste)
    {
        List<Medecin?> l_medecin = new List<Medecin?>();
        try
        {
            //si l'utilisateur n'a encore rien entrer et rien séléctionner pour le poste: on recupère tout
            if((value == null || value == "" || value.Trim() == "") && poste == null)
            {
                var medecins = await _medecinrepository.GetAllAsync();
                l_medecin = medecins.ToList();
            }
            //si le poste n'est pas séléctionner: on recupère par like
            else if(poste == null && value != null)
            {
                var medecins = await _medecinrepository.GetLikeAsync(value.Trim());
                l_medecin = medecins.ToList();
            }
            //si le poste seule est séléctionner: on recupere par poste
            else if(value == null && poste != null)
            {
                var medecins = await _medecinrepository.GetPosAsync(poste);
                l_medecin = medecins.ToList();
            }
            //Cas de double recherche
            else if(value != null && poste != null)
            {
                var medecins = await _medecinrepository.GetLikePosteAsync(value.Trim(),poste);
                l_medecin = medecins.ToList();
            }
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
        }
        return l_medecin;
    }

    public async Task<Medecin?> GetMedecinAsync(string? codemed)
    {
        try
        {
            if(codemed != null)
            {
                return await _medecinrepository.GetByIdAsync(codemed);
            }
            return null;
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return null;
        }
    }

    public async Task<(bool Success, string Message)> SupprimerMedecin(string codemed)
    {
        try
        {
            if(await _medecinrepository.DeleteAsync(codemed) && _usersrepository != null)
            {
                if(await _usersrepository.DeleteAsync(codemed)) return (true,"Medecin Supprimé avec succès");
            }
            return (false,"Erreur de suppression du medecin!");
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return (false,$"Error: {exc.Message}");
        }
    }

    public async Task<(bool Success, string Message)> MettreJourMedecin(string codemed, string? nommed, string? poste)
    {
        try
        {
            if(nommed == null || nommed == "" || nommed.Trim() == "") return (false,"Nom obligatoire");
            if(poste == null) return (false,"Séléctionner uhn poste");
            Medecin? medecin = new Medecin(null,codemed,nommed,poste);
            if(await _medecinrepository.UpdateAsync(medecin)) return (true,"Mis à jour avec succès");
            return (false,"Mis à jour non éffectué");
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return (false,$"Error: {exc.Message}");
        }
    }
}
