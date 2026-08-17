using GestionHospitaliere.Backend.Interfaces;
using GestionHospitaliere.Backend.Interfaces.Services;
using GestionHospitaliere.Backend.Models;

namespace GestionHospitaliere.Backend.Services;

public class ChambreServices : IChambreServices
{
    private IChambreRepository _chambrerepository;

    public ChambreServices(IChambreRepository chambrerepository)
    {
        _chambrerepository = chambrerepository;
    }

    public async Task<List<Chambre?>> ListeChambre()
    {
        try
        {
            var chambres = await _chambrerepository.GetAllAsync();
            return chambres.ToList() ?? [];
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return [];
        }
    }

    public async Task<Chambre?> RechercheChambre(int numchambre)
    {
        try
        {
            if(numchambre > 0)
            {
                return await _chambrerepository.GetByIdAsync(numchambre) ?? null;
            }
            return null;
        }
        catch(Exception exc)
        {
            Console.WriteLine($"Error: {exc.Message}");
            return null;
        }
    }

}
